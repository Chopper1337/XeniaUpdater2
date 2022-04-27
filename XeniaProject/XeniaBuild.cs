using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XeniaProject
{
    public class XeniaBuild
    {
        public XeniaBuild(string Name, string URL, string Description, string FolderName, string ImagePath, Stability StabilityRating, string ExecutableName )
        {
            this.Name = Name;
            this.URL = URL;
            this.Description = Description;
            this.FolderName = FolderName;
            this.ImagePath = ImagePath;
            this.StabilityRating = StabilityRating;
            this.ExecutableZipName = ExecutableName;
        }
        public string Name { get; set; }
        public string URL { get; set; }
        public string Description { get; set; }
        public string FolderName { get; set; }
        public string ImagePath { get; set; }
        public Stability StabilityRating { get; set; }
        public string ExecutableZipName { get; set; }

        public enum Stability
        {
            Stable,
            Unstable,
            Unsafe
        }

        public override string ToString()
        {
            return $"Xenia {Name}";
        }

    }
}
