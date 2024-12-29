namespace SRDebugger.Services.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SRF;
    using SRF.Service;
    using UnityEngine;

    /// <summary>
    /// Reports system specifications and environment information. Information that can
    /// be used to identify a user is marked as private, and won't be included in any generated
    /// reports.
    /// </summary>
    [Service(typeof(IGMInformationService))]
    public class StandardGMInformationService : IGMInformationService
    {
        private readonly Dictionary<string, IList<InfoEntry>> _info = new Dictionary<string, IList<InfoEntry>>();
        public static System.Func<String> GetTotalResVersionCallback;
        public static System.Func<String> GetSmallGameResVersionCallback;
        public static System.Func<String> GetAOTResVersionCallback;
        public static System.Func<String> GetHotFixVersionCallback;
        public static System.Func<String> GetUserLayerCallback;
        public static System.Func<String> GetHotFixStateCallback;
        public static System.Func<String> GetCanvasCallback;
        public static System.Func<String> GetSafeAreaCallback;
        public static System.Func<String> GetCountryCallback;
        public static System.Func<String> GetCityCallback;
        public static System.Func<String> GetJenkinsIdCallback;
        
        public StandardGMInformationService()
        {
            CreateDefaultSet();
        }

        public IEnumerable<string> GetCategories()
        {
            return _info.Keys;
        }

        public IList<InfoEntry> GetInfo(string category)
        {
            IList<InfoEntry> list;

            if (!_info.TryGetValue(category, out list))
            {
                Debug.LogError("[SystemInformationService] Category not found: {0}".Fmt(category));
                return new InfoEntry[0];
            }

            return list;
        }

        public void Add(InfoEntry info, string category = "Default")
        {
            IList<InfoEntry> list;

            if (!_info.TryGetValue(category, out list))
            {
                list = new List<InfoEntry>();
                _info.Add(category, list);
            }

            if (list.Any(p => p.Title == info.Title))
            {
                throw new ArgumentException("An InfoEntry object with the same title already exists in that category.", "info");
            }

            list.Add(info);
        }

        public Dictionary<string, Dictionary<string, object>> CreateReport(bool includePrivate = false)
        {
            var dict = new Dictionary<string, Dictionary<string, object>>();

            foreach (var keyValuePair in _info)
            {
                var category = new Dictionary<string, object>();

                foreach (var systemInfo in keyValuePair.Value)
                {
                    if (systemInfo.IsPrivate && !includePrivate)
                    {
                        continue;
                    }

                    category.Add(systemInfo.Title, systemInfo.Value);
                }

                dict.Add(keyValuePair.Key, category);
            }

            return dict;
        }

        private void CreateDefaultSet()
        {
            _info.Add("游戏信息", new[]
            {
                InfoEntry.Create("总体资源号", GetTotalResVersionCallback),
                InfoEntry.Create("小游戏资源号", GetSmallGameResVersionCallback),
                InfoEntry.Create("AOT资源号", GetAOTResVersionCallback),
                InfoEntry.Create("热更版本号", GetHotFixVersionCallback),
                InfoEntry.Create("是否热更新", GetHotFixStateCallback),
                InfoEntry.Create("用户层", GetUserLayerCallback),
                InfoEntry.Create("画布", GetCanvasCallback),
                InfoEntry.Create("安全区域", GetSafeAreaCallback),
                InfoEntry.Create("国家", GetCountryCallback),
                InfoEntry.Create("城市", GetCityCallback),
                InfoEntry.Create("JenkinsId", GetJenkinsIdCallback)
            });
        }

        private static string GetCloudManifestPrettyName(string name)
        {
            switch (name)
            {
                case "scmCommitId":
                    return "Commit";

                case "scmBranch":
                    return "Branch";

                case "cloudBuildTargetName":
                    return "Build Target";

                case "buildStartTime":
                    return "Build Date";
            }

            // Return name with first letter capitalised
            return name.Substring(0, 1).ToUpper() + name.Substring(1);
        }
    }
}
