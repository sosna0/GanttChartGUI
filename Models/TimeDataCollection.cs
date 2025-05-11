using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models {
    public class TimeDataCollection : ObservableCollection<TimeOnly> {
        public TimeDataCollection() {
            for (int i = 0; i < 25; i++) {
                Add(new TimeOnly(i%24, 0));
            }
        }
    }
}
