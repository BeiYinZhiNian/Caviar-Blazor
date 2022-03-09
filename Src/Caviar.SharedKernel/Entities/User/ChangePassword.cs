using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "RequiredErrorMsg")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "RequiredErrorMsg")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "RequiredErrorMsg")]
        public string ConfirmPassword { get; set; }
    }
}
