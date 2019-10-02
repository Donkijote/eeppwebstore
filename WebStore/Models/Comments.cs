using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebStore.Models
{
    public class Questions
    {
        public string Question { get; set; }
        public DateTime QuestionDate { get; set; }
        public Answers Answer { get; set; }
        public int User { get; set; }
    }

    public class Answers
    {
        public string Answer { get; set; }
        public DateTime AnswerDate { get; set; }
    }
}