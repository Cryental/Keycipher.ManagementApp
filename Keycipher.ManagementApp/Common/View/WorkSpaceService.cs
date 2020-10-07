using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Interactivity;
using Keycipher.ManagementApp.Common.ViewModel;

namespace Keycipher.ManagementApp.Common.View
{
    public class WorkspaceService : Decorator, IWorkspaceService
    {
        public static readonly RoutedEvent WorkspaceRegionRegisterEvent =
            EventManager.RegisterRoutedEvent("WorkspaceRegionRegister", RoutingStrategy.Bubble,
                typeof(WorkspaceRegionEventHandler), typeof(WorkspaceService));

        public static readonly RoutedEvent WorkspaceRegionUnregisterEvent =
            EventManager.RegisterRoutedEvent("WorkspaceRegionUnregister", RoutingStrategy.Bubble,
                typeof(WorkspaceRegionEventHandler), typeof(WorkspaceService));

        private readonly Dictionary<string, IWorkspaceRegion> regions = new Dictionary<string, IWorkspaceRegion>();
        private Workspace workspace = new Workspace();
        private bool workspaceChanging;

        public WorkspaceService()
        {
            AddHandler(WorkspaceRegionRegisterEvent, new WorkspaceRegionEventHandler(OnWorkspaceRegionRegister));
            AddHandler(WorkspaceRegionUnregisterEvent, new WorkspaceRegionEventHandler(OnWorkspaceRegionUnregister));
            Interaction.GetBehaviors(this).Add(new WorkspaceServiceInternal(this));
        }

        public Workspace SaveWorkspace()
        {
            if (workspaceChanging)
            {
                throw new InvalidOperationException();
            }

            workspaceChanging = true;
            workspace = new Workspace();
            foreach (var region in regions.Values)
            {
                workspace.AddRegion(region.Id, region.SaveLayout());
            }

            return workspace;
        }

        public void RestoreWorkspace(Workspace workspace)
        {
            if (!workspaceChanging)
            {
                throw new InvalidOperationException();
            }

            this.workspace = workspace;
            foreach (var region in regions.Values)
            {
                SyncWorkspaceRegionLayout(region);
            }

            workspaceChanging = false;
        }

        private void OnWorkspaceRegionRegister(object sender, WorkspaceRegionEventArgs e)
        {
            e.Handled = true;
            if (!regions.ContainsKey(e.Region.Id))
            {
                regions.Add(e.Region.Id, e.Region);
            }

            if (!workspaceChanging)
            {
                SyncWorkspaceRegionLayout(e.Region);
            }
        }

        private void OnWorkspaceRegionUnregister(object sender, WorkspaceRegionEventArgs e)
        {
            e.Handled = true;
            regions.Remove(e.Region.Id);
        }

        private void SyncWorkspaceRegionLayout(IWorkspaceRegion region)
        {
            var regionLayout = workspace.FindRegionLayout(region.Id);
            if (regionLayout != null)
            {
                region.RestoreLayout(regionLayout);
            }
            else
            {
                workspace.AddRegion(region.Id, region.SaveLayout());
            }
        }

        private class WorkspaceServiceInternal : ServiceBase, IWorkspaceService
        {
            private readonly IWorkspaceService workspaceService;

            public WorkspaceServiceInternal(IWorkspaceService workspace)
            {
                workspaceService = workspace;
            }

            public Workspace SaveWorkspace()
            {
                return workspaceService.SaveWorkspace();
            }

            public void RestoreWorkspace(Workspace workspace)
            {
                workspaceService.RestoreWorkspace(workspace);
            }
        }
    }

    public interface IWorkspaceRegion
    {
        string Id { get; }
        string SaveLayout();
        void RestoreLayout(string layout);
    }

    public class WorkspaceRegionEventArgs : RoutedEventArgs
    {
        public WorkspaceRegionEventArgs(IWorkspaceRegion region)
        {
            Region = region;
        }

        public IWorkspaceRegion Region { get; }
    }

    public delegate void WorkspaceRegionEventHandler(object sender, WorkspaceRegionEventArgs e);

    public class WorkspaceRegionBehavior : Behavior<FrameworkElement>, IWorkspaceRegion
    {
        #region Dependency Properties

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(string), typeof(WorkspaceRegionBehavior),
                new PropertyMetadata(null));

        #endregion

        private MethodInfo restoreLayoutFromStreamMethod;
        private MethodInfo saveLayoutToStreamMethod;

        public string Id
        {
            get => (string) GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }

        private MethodInfo RestoreLayoutFromStreamMethod
        {
            get
            {
                if (restoreLayoutFromStreamMethod == null)
                {
                    restoreLayoutFromStreamMethod = GetMethod(AssociatedObject.GetType(), "RestoreLayoutFromStream");
                }

                return restoreLayoutFromStreamMethod;
            }
        }

        private MethodInfo SaveLayoutToStreamMethod
        {
            get
            {
                if (saveLayoutToStreamMethod == null)
                {
                    saveLayoutToStreamMethod = GetMethod(AssociatedObject.GetType(), "SaveLayoutToStream");
                }

                return saveLayoutToStreamMethod;
            }
        }

        string IWorkspaceRegion.Id => Id;

        string IWorkspaceRegion.SaveLayout()
        {
            using (var stream = new MemoryStream())
            {
                SaveLayoutToStream(stream);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        void IWorkspaceRegion.RestoreLayout(string layout)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(layout)))
            {
                RestoreLayoutFromStream(stream);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnAssociatedObjectLoaded;
            AssociatedObject.Unloaded += OnAssociatedObjectUnloaded;
            if (AssociatedObject.IsLoaded)
            {
                OnAssociatedObjectLoaded(AssociatedObject, null);
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= OnAssociatedObjectLoaded;
            AssociatedObject.Unloaded -= OnAssociatedObjectUnloaded;
            if (AssociatedObject.IsLoaded)
            {
                OnAssociatedObjectUnloaded(AssociatedObject, null);
            }
        }

        private void OnAssociatedObjectUnloaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.RaiseEvent(new WorkspaceRegionEventArgs(this)
            {
                RoutedEvent = WorkspaceService.WorkspaceRegionUnregisterEvent
            });
        }

        private void OnAssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.RaiseEvent(
                new WorkspaceRegionEventArgs(this) {RoutedEvent = WorkspaceService.WorkspaceRegionRegisterEvent});
        }

        private MethodInfo GetMethod(Type type, string methodName)
        {
            for (; type != null; type = type.BaseType)
            {
                var method = type.GetMethod(methodName);
                if (method != null)
                {
                    return method;
                }
            }

            throw new InvalidOperationException();
        }

        private void SaveLayoutToStream(Stream stream)
        {
            SaveLayoutToStreamMethod.Invoke(AssociatedObject, new object[] {stream});
        }

        private void RestoreLayoutFromStream(Stream stream)
        {
            RestoreLayoutFromStreamMethod.Invoke(AssociatedObject, new object[] {stream});
        }
    }
}