using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using WebStore.Validators;

namespace WebStore.Models
{
    public class AvatarsForm
    {
        [Required(ErrorMessage = "Campo requerido")]
        [ValidFileTypeValidator("png", "jpg")]
        [MaximumFileSizeValidator(1)]
        public HttpPostedFileBase AvatarFile { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class ProfileMenu
    {
        public AvatarsForm AvatarsForm { get; set; }
        public List<Avatars> Avatars { get; set; }
    }
}