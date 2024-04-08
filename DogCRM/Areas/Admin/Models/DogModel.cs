using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DogCRM.Areas.Admin.Models
{
    public class DogModel
    {
        public int Id {get; set;}
        [Required]
        public string Breed { get; set; }
        [Required]
        public string Name {get; set; }
        [Required]
        public string Size { get; set; }

        [Required]
        public string Age { get; set; }
        [Required]
        public string Gender { get; set; }

        [Display(Name = "Photo")]
        public string ImagesUrl {get; set; }
        
        [Required(ErrorMessage = "Please select file.")]
        //[FileExtensions(Extensions = "jpg,jpeg,png,gif")]
        public HttpPostedFileBase[] files { get; set; }

        [Display(Name = "Video")]
        public string VideosUrl { get; set; }
        
        public HttpPostedFileBase[] Video { get; set; }
        [Required]
        public string Characteristics { get; set;}
       
        [Required]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Video Title")]
        public string videoTitle { get; set; }
        [Required]
        [Display(Name = "Video Description")]
        public string videoDescription { get; set; }
        public string Tags { get; set; }

    }
    public enum DogGender
    {
        Male=1,
        Female=2
    }

    public class YouTubeVideoObject
    {
        public string VideoId { get; set; }
        public string Title { get; set; }
    }
    public class UploadModel
    {
        public HttpPostedFileBase[] ImageList { get; set; }
        public string imgMessage { get; set; }
        public bool imgValid { get; set; }
        public HttpPostedFileBase[] VideoList { get; set; }
        public string vdoMessage { get; set; }
        public bool vdoValid { get; set; }
        public string cancelMessage { get; set; }
    }
}