// using WorldDiabetesFoundation.Core.Domain.DataTransferObjects;
// using WorldDiabetesFoundation.Core.Infrastructure.UmbracoTools;
//
// namespace WorldDiabetesFoundation.Core.Infrastructure.DataSeeder;
//
// public class ProjectSeeder
// {
//
//     private readonly UmbracoDataImporter _dataImporter;
//
//     public ProjectSeeder(UmbracoDataImporter dataImporter)
//     {
//         _dataImporter = dataImporter;
//         GenerateProjects();
//     }
//
//     private void GenerateProjects()
//     {
//         try
//         {
//             var list = GenerateProjectList();
//             var formattedList = new List<ProjectDTO>();
//
//             foreach (var entity in list)
//             {
//                 FormatConverter.ConvertToTitleCaseProjectDto(entity);
//                 formattedList.Add(entity);
//             }
//             
//             _dataImporter.ProcessAndStoreProjects(formattedList);
//             _dataImporter.PublishAndReleaseAllProjects();
//         }
//         catch(Exception e) {
//             Console.WriteLine(e);
//         }
//         
//     }
//     
//     private readonly Dictionary<string, List<(string Country, (double Latitude, double Longitude) Coordinates)>> _regionCountriesMapping = new()
//     {
//         { "North America", new List<(string, (double, double))> { ("USA", (38.8951, -77.0364)), ("Canada", (45.4215, -75.6972)), ("Mexico", (19.4326, -99.1332)) } },
//         { "South America", new List<(string, (double, double))> { ("Brazil", (-15.8267, -47.9218)), ("Argentina", (-34.6037, -58.3816)), ("Colombia", (4.7110, -74.0721)) } },
//         { "Europe", new List<(string, (double, double))> { ("France", (48.8566, 2.3522)), ("Germany", (52.5200, 13.4050)), ("UK", (51.5074, -0.1278)), ("Spain", (40.4168, -3.7038)), ("Italy", (41.9028, 12.4964)) } },
//         { "Asia", new List<(string, (double, double))> { ("China", (39.9042, 116.4074)), ("India", (28.6139, 77.2090)), ("Japan", (35.6895, 139.6917)), ("South Korea", (37.5665, 126.9780)), ("Indonesia", (-6.2088, 106.8456)) } },
//         { "Africa", new List<(string, (double, double))> { ("Nigeria", (9.0820, 7.4920)), ("South Africa", (-25.7464, 28.1887)), ("Egypt", (30.0444, 31.2357)), ("Kenya", (-1.2921, 36.8219)), ("Ethiopia", (9.1450, 40.4897)) } },
//         { "Australia", new List<(string, (double, double))> { ("Australia", (-35.2809, 149.1300)), ("New Zealand", (-41.2865, 174.7762)) } },
//         { "Middle East", new List<(string, (double, double))> { ("Saudi Arabia", (24.7136, 46.6753)), ("Iran", (35.6892, 51.3890)), ("UAE", (24.4539, 54.3773)), ("Qatar", (25.276987, 51.520008)), ("Israel", (31.7683, 35.2137)) } },
//         { "Central America", new List<(string, (double, double))> { ("Guatemala", (14.6349, -90.5069)), ("Costa Rica", (9.9281, -84.0907)), ("Panama", (8.9806, -79.5188)) } },
//         { "Caribbean", new List<(string, (double, double))> { ("Cuba", (23.1136, -82.3666)), ("Jamaica", (18.0179, -76.8099)), ("Dominican Republic", (18.4861, -69.9312)) } },
//         { "Oceania", new List<(string, (double, double))> { ("Fiji", (-18.1248, 178.4501)), ("Papua New Guinea", (-9.4438, 147.1803)), ("Samoa", (-13.8333, -171.7500)) } }
//     };
//     
//     private readonly string[] _diabetesTerms = new[]
//     {
//         "insulin", "glucose", "monitoring", "blood sugar", "treatment", "prevention", "awareness",
//         "care", "complication", "mellitus", "type 1", "type 2", "gestational", "nutrition",
//         "exercise", "lifestyle", "management", "education", "community", "therapy", "technology"
//     };
//
//     private string GenerateRandomDiabetesDescription()
//     {
//         Random rand = new Random();
//         int numberOfTerms = rand.Next(50, 60); // Choose between 5 to 10 terms
//         List<string> selectedTerms = new List<string>();
//
//         for (int i = 0; i < numberOfTerms; i++)
//         {
//             int randomIndex = rand.Next(0, _diabetesTerms.Length);
//             selectedTerms.Add(_diabetesTerms[randomIndex]);
//         }
//
//         return string.Join(" ", selectedTerms);
//     }
//     
//     private readonly string[] _interventionAreas = new[]
//     {
//         "Type 1 Diabetes Care",
//         "Type 2 Diabetes Management",
//         "Gestational Diabetes",
//         "Diabetes and Mental Health",
//         "Nutrition and Diabetes",
//         "Exercise and Diabetes",
//         "Insulin Therapy",
//         "Blood Sugar Monitoring",
//         "Diabetes in Youth",
//         "Diabetes in the Elderly",
//         "Diabetic Foot Care",
//         "Diabetic Retinopathy",
//         "Cardiovascular Risks",
//         "Renal Complications",
//         "Diabetic Neuropathy",
//         "Diabetes Education",
//         "Community Outreach",
//         "Telemedicine for Diabetes",
//         "Diabetes and Pregnancy",
//         "Precision Medicine in Diabetes"
//     };
//     
//     private List<ProjectDTO> GenerateProjectList()
// {
//     var projects = new List<ProjectDTO>();
//     Random random = new Random();
//
//     for (int i = 1; i <= 100; i++)
//     {
//         var numberOfRegions = random.Next(1, 4);
//         List<string> regionList = new List<string>();
//         List<string> countryList = new List<string>();
//         List<string> locationList = new List<string>();
//
//         for (int j = 0; j < numberOfRegions; j++)
//         {
//             var randomRegionIndex = random.Next(0, _regionCountriesMapping.Keys.Count);
//             var region = _regionCountriesMapping.Keys.ElementAt(randomRegionIndex);
//             var countries = _regionCountriesMapping[region];
//             var randomCountryIndex = random.Next(0, countries.Count);
//             var (country, (lat, lon)) = countries[randomCountryIndex];
//
//             // Add random variance to the coordinates
//             double variance = 0.8;  // You can adjust the variance level
//             double randomLat = lat + (random.NextDouble() * 2 - 1) * variance;
//             double randomLon = lon + (random.NextDouble() * 2 - 1) * variance;
//
//             regionList.Add(region);
//             countryList.Add(country);
//             locationList.Add($"{randomLat},{randomLon}");
//         }
//
//         // Join the lists into single strings
//         string regionsStr = string.Join(";", regionList);
//         string countriesStr = string.Join(";", countryList);
//         string locationsStr = string.Join(";", locationList);
//         
//         int startYear = random.Next(2000, 2050);
//         int endYear = random.Next(2000, 2050);
//
//         if (startYear > endYear)
//         {
//             int temp = startYear;
//             startYear = endYear;
//             endYear = temp;
//         }
//         
//         // Randomly select 1 to 3 intervention areas for this project
//         var selectedInterventionAreas = _interventionAreas.OrderBy(x => random.Next()).Take(random.Next(1, 4)).ToList();
//
//         var project = new ProjectDTO
//         {
//             title = $"Project Title {i}",
//             project_id = $"P{i:D5}",
//             status = i % 2 == 0 ? "Active" : "Inactive",
//             intervention_areas = string.Join(";", selectedInterventionAreas),
//             regions = regionsStr,  // Updated to fit a normal string
//             countries = countriesStr, // Updated to fit a normal string
//             period = $"{startYear} to {endYear}",
//             locations = locationsStr,  // Updated to fit a normal string
//             partners = "Partner A, Partner B",
//             budget = $"{random.Next(100000, 500000)} USD",
//             wdf_contribution = $"{random.Next(5000, 20000)} USD",
//             notes = "This is a test project.",
//             objectives = GenerateRandomDiabetesDescription(),
//             approach = GenerateRandomDiabetesDescription(),
//             expected_results = GenerateRandomDiabetesDescription(),
//             results_upon_completion = i % 3 == 0 ? "Successful" : "Ongoing"
//         };
//
//         projects.Add(project);
//     }
//
//     return projects;
// }
//
//
// }