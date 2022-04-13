using System.ComponentModel.DataAnnotations;

namespace Caviar.SharedKernel.Entities
{
    public partial class SysMenu : SysUseEntity
    {

        [Required]
        [StringLength(50)]
        public string MenuName { get; set; }

        public MenuType MenuType { get; set; }

        public TargetType TargetType { get; set; }

        [StringLength(1024)]
        public string Url { get; set; }

        [StringLength(50)]
        public string Icon { get; set; }

        public int ParentId { get; set; }

        public ButtonPosition ButtonPosition { get; set; }

        public bool IsDoubleTrue { get; set; }

        [StringLength(50)]
        public string HttpMethods { get; set; }
    }


}
