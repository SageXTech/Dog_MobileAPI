using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DogCRM.Areas.Admin.Models
{
    public class EventModel
    {
        public int Id { get; set; }
        [Display(Name = "Photo")]
        public string ImagesUrl { get; set; }
        [Required(ErrorMessage = "Please select file.")]
        [MaxLength(4, ErrorMessage = "selected cannot be greater than 4")]
        //[FileExtensions(Extensions = "jpg,jpeg,png,gif")]
        //[FileExtensions("jpg,jpeg,png,gif", ErrorMessage = "please select only image file.")]
        public HttpPostedFileBase[] PostedImage { get; set; }
        [Display(Name = "Video")]
        public string VideosUrl { get; set; }
        [MaxLength(1,ErrorMessage = "selected cannot be greater than 1")]
        public HttpPostedFileBase[] PostedVideo { get; set; }
        [Required(ErrorMessage = "Please select event name.")]
        [Display(Name = "Event Name")]
        public string EventName { get; set; }
        [Required(ErrorMessage = "Please select Location.")]
        public string Location { get; set; }
        [Required(ErrorMessage = "Please Enter Address.")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Please Enter Description.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please select EventType.")]
        public int? EventType { get; set; }
        public string EventTypeName { get; set; }

        [Required(ErrorMessage = "Plese Enter Event Price")]
        public decimal? EventPrice { get; set; }
        [Required(ErrorMessage = "Please select start date-time.")]
        [Display(Name = "Start Date-Time")]
        public DateTime? StartDateTime { get; set; }
        [Required(ErrorMessage = "Please select end date-time.")]
        [Display(Name = "End Date-Time")]
        public DateTime? EndDateTime { get; set; }
        [Required(ErrorMessage = "Please enter video Title.")]
        [Display(Name = "Video Title")]
        public string videoTitle { get; set; }
        [Required(ErrorMessage = "Please enter video description.")]
        [Display(Name = "Video Description")]
        public string videoDescription { get; set; }
        public string Tags { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
    }
}