using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DogAPI.Models
{
    public class DogModel
    {
        public int Id { get; set; }
        [Required]
        public string Breed { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Size { get; set; }
        [Required]
        public string Age { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public List<string> ImagesUrl { get; set; }
        public List<string> VideosUrl { get; set; }
        [Required]
        public string Characteristics { get; set; }
        [Required]
        public string Description { get; set; }
    }
    public class ImageVm
    {
        public string Name { get; set; }
    }
    public class VideoVm
    {
        public string Name { get; set; }
    }
    public class DogFilter
    {
        public int Id { get; set; }
        public string Breed { get; set; }
        public string Name { get; set; }
        public string ImagesUrl { get; set; }
        public string VideosUrl { get; set; }
    }
    public enum DogGender
    {
        Male = 1,
        Female = 2
    }
    public class DogProfile
    {
        [Required]
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public int? Gender { get; set; }
        public string Photo { get; set; }
    }
  
    public enum ErrorModel
    {
        [Display(Name ="Data are not found.")]
        NotFound,
        [Display(Name ="Id must be grether then 0(zero).")]
        IdErr,
        [Display(Name ="Data updated successfuly.")]
        Success
    }

}