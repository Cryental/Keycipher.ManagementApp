using System.Collections.Generic;

namespace Keycipher.ManagementApp.Common.ViewModel
{
    public class Workspace
    {
        private readonly Dictionary<string, string> regions = new Dictionary<string, string>();
        public IEnumerable<KeyValuePair<string, string>> Regions => regions;

        public void AddRegion(string regionId, string regionLayout)
        {
            regions.Add(regionId, regionLayout);
        }

        public string FindRegionLayout(string regionId)
        {
            string regionLayout = null;
            return regions.TryGetValue(regionId, out regionLayout) ? regionLayout : null;
        }
    }

    public interface IWorkspaceService
    {
        Workspace SaveWorkspace();
        void RestoreWorkspace(Workspace workspace);
    }
}