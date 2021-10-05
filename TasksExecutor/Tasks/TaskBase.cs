using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksExecutor.Tasks
{
    class TaskBase
    {
        protected TaskBase(string url, IEnumerable<int> bookingIds, Regions region)
        {
            TaskUrl = url;
            BookingIds = bookingIds;
            Region = region;
            TagRegion = RegionToTag(Region);
            TagDivision = "Бронирование";
            TaskTime = "5";
        }

        public IEnumerable<int> BookingIds { get; }
        public Regions Region { get; }
        public string TagAction { get; init; }
        public string TagDivision { get; }
        public string TagRegion { get; }
        public string TaskTime { get; }
        public string TaskUrl { get; }

        string RegionToTag(Regions region) =>
            region switch
            {
                Regions.BlackSeaCoast => "Черноморское побережье",
                Regions.Crymea => "Крым",
                Regions.Ekaterinburg => "Екатеринбург",
                Regions.Izhevsk => "Ижевск",
                Regions.Kaliningrad => "Калининград",
                Regions.Kazan => "Казань",
                Regions.Kirov => "Киров",
                Regions.Krasnodar => "Краснодар",
                Regions.Moscow => "Москва",
                Regions.Novosibirsk => "Новосибирск",
                Regions.Penza => "Пенза",
                Regions.Perm => "Пермь",
                Regions.SaintPetersburg => "СПБ",
                Regions.Samara => "Самара",
                Regions.Chelyabinsk => "Челябинск",
                Regions.Vladimir => "Владимир",
                Regions.Vladivostok => "Владивосток",
                Regions.Voronezh => "Воронеж",
                _ => ""
            };
    }
}

