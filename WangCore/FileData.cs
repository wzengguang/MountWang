using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Wang.Setting
{
    public class FileData
    {
        private static string FILE_PATH = BasePath.Name + "Modules/WangSetting.json";


        public static List<ISetting> Settings { get; set; } = new List<ISetting>();



        public static void ReadFromFile(Type module)
        {

            if (!File.Exists(FILE_PATH))
            {
                // Create the file.
                using (FileStream fs = File.Create(FILE_PATH))
                {
                }
            }
            try
            {
                using (StreamReader file = File.OpenText(FILE_PATH))
                {
                    var str = file.ReadToEnd();
                    if (!string.IsNullOrEmpty(str))
                    {
                        JObject keyValues = JObject.Parse(str);
                        foreach (var item in keyValues)
                        {
                            try
                            {
                                var type = module.Assembly.GetType(item.Key);
                                if (type == null || Settings.FirstOrDefault(a => a.GetType() == type) != null)
                                {
                                    continue;
                                }
                                var obj = Activator.CreateInstance(type) as ISetting;
                                JsonConvert.PopulateObject(item.Value.ToString(), obj);
                                Settings.Add(obj);
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.FlattenException());
                            }
                        }
                    }

                }


            }
            catch (Exception e)
            {
                MessageBox.Show(e.FlattenException());
            }


            SyncFromAssemble(module);
        }

        private static void SyncFromAssemble(Type module)
        {

            Assembly assembly = module.Assembly;
            var types = assembly.GetTypes().Where(a => a != typeof(SettingBase) && typeof(SettingBase).IsAssignableFrom(a)).ToList();


            foreach (var item in types)
            {
                if (Settings.FirstOrDefault(a => a.GetType() == item) == null)
                {
                    Settings.Add(Activator.CreateInstance(item) as ISetting);
                }
            }
        }

        public async static Task SaveToFile()
        {
            Dictionary<string, ISetting> pairs = new Dictionary<string, ISetting>();
            foreach (var item in Settings)
            {
                pairs.Add(item.GetType().FullName, item);
            }

            await Task.Run(() =>
            {
                try
                {
                    File.WriteAllText(FILE_PATH, JsonConvert.SerializeObject(pairs));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.FlattenException());
                }
            }).ConfigureAwait(false);
        }
    }




}
