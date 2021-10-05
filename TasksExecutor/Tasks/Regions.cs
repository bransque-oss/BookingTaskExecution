using System;

namespace TasksExecutor.Tasks
{
    public enum Regions : byte
    {
        BlackSeaCoast,
        Chelyabinsk,
        Crymea,
        Ekaterinburg,
        Izhevsk,
        Kazan,
        Kaliningrad,
        Kirov,
        Krasnodar,
        Moscow,
        Novosibirsk,
        Penza,
        Perm,
        SaintPetersburg,
        Samara,
        Vladimir,
        Vladivostok,
        Voronezh
    }
    public static class RegionsExtension
    {
        public static Regions StringToRegions(this String regionName) =>
            regionName.ToLower() switch
            {
                "владимир" => Regions.Vladimir,
                "владивосток" => Regions.Vladivostok,
                "воронеж" => Regions.Voronezh,
                "екатеринбург" => Regions.Ekaterinburg,
                "ижевск" => Regions.Izhevsk,
                "казань" => Regions.Kazan,
                "калининград" => Regions.Kaliningrad,
                "киров" => Regions.Kirov,
                "краснодар" => Regions.Krasnodar,
                "крым" => Regions.Crymea,
                "москва" => Regions.Moscow,
                "новосибирск" => Regions.Novosibirsk,
                "пенза" => Regions.Penza,
                "пермь" => Regions.Perm,
                "самара" => Regions.Samara,
                "санкт-петербург" => Regions.SaintPetersburg,
                "челябинск" => Regions.Chelyabinsk,
                "черноморское побережье" => Regions.BlackSeaCoast,
                _ => throw new Exception("Регион указанный в задаче не существует")
            };
    }
}
