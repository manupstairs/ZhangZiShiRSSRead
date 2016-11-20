using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhangZiShiRssRead
{
    public class Item //: ViewModelBase
    {
        public string Title { get; set; }

        public Uri Link { get; set; }

        public DateTime PublishedDate { get; set; }

        public string Creator { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        //public Content ItemContent { get; set; }

        public string ContentEncoded { get; set; }

        public string CoverImageUri { get; set; }

        
    }
}
