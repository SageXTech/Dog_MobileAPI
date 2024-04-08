using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;

namespace DogAPI.Models
{
    public class EventModel
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string[] Location { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public List<string> ImagesUrl { get; set; }
        public List<string> VideosUrl { get; set; }
        public string EventType { get; set; }
        public decimal? EventPrice { get; set; }
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public bool EventInterested { get; set; }
        public int EventInterestedCount { get; set; }
    }

    public class EventShortModel
    {
        public int Id { get; set; }
        public string EventType { get; set; }
        public string EventName { get; set; }
        public string Address { get; set; }
        public string ImagesUrl { get; set; }
        public decimal? EventPrice { get; set; }
        public string StartDateTime { get; set; }
    }
    public class EventInterestedModel
    {
        [Required]
        public int? EventId { get; set; }
        [Required]
        public int? UserId { get; set; }
    }
    public class EventFilter
    {
        public List<int> EventType { get; set; }
        public List<int> EventPrice { get; set; }
        public List<int> EventDate { get; set; }
    }
    public class EventMultipleList
    {
        public List<itemDetails> categoryList { get; set; }
        public List<itemDetails> priceList { get; set; }
        public List<itemDetails> dateList { get; set; }
    }
    public class itemDetails
    {
        public string Text { get; set; }
        public int Value { get; set; }
        public bool Selected { get; set; }
    }

    public class EventShortFilter
    {
        public int? Id { get; set; }
        public int? EventType { get; set; }
        public string EventName { get; set; }
        public string Address { get; set; }
        public string ImagesUrl { get; set; }
        public decimal? EventPrice { get; set; }
        public DateTime? StartDateTime { get; set; }
    }
   
}