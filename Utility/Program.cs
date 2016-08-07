using System;
using System.Reflection;
using DarkMultiPlayerServer;
using System.ComponentModel;
using System.Linq;

namespace DarkMultiPlayer.Utility
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Available commands: ");
                Console.WriteLine("\t--docs");
                Console.WriteLine();
                return;
            }

            if (args[0] == "--docs")
            {
                try
                {
                    SettingsStore settings = new SettingsStore();
                    FieldInfo[] settingFields = typeof(SettingsStore).GetFields();
                    foreach (FieldInfo settingField in settingFields)
                    {
                        object descAttr = settingField.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
                        string description = descAttr != null ? ((DescriptionAttribute)descAttr).Description.
                            Replace("\n", string.Format("\t{0}", Environment.NewLine)).Replace("# ", "") : string.Empty;

                        if (!string.IsNullOrEmpty(description))
                        {
                            Console.WriteLine("## {0}", settingField.Name);
                            Console.WriteLine(description);
                            Console.WriteLine();
                            Console.WriteLine("**Default Value:** {0}", settingField.GetValue(settings).ToString());
                            Console.WriteLine();
                            if (settingField.FieldType.IsEnum)
                            {
                                Console.WriteLine("Valid values are:");
                                foreach (object enumValue in settingField.FieldType.GetEnumValues())
                                    Console.WriteLine("- {0}", enumValue.ToString());
                                Console.WriteLine();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
    }
}
