using PlantPod.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlantPod.Data
{
    public class Arduino
    {
        private int id { get; set; }
        private List<string> Labels;
        private List<decimal> Data;
        private List<ReadingModel> readings;
        private int user_id { get; set; }
        private string plantName { get; set; }

        private DataLibrary.IDataAccess _data;
        public Arduino(DataLibrary.IDataAccess dataAccess) => _data = dataAccess;

        public async Task<string> getPlantName(int user_id)
        {
            // pull plantdata if it doesn't exist otherwise return plantname
            if (plantName == null && user_id != 0)
            {
                await pullPlantData(user_id);
            }
            return plantName;
        }

        public async Task pullPlantData(int user_id)
        {
            // Get arduino with given user id
            List<ArduinoModel> arduinos = await _data.LoadData<ArduinoModel, dynamic>("select plant_name, id from arduino where user_id=@user_id", new { user_id = user_id }, ConnectionData.ConnectionString);

            if (arduinos != null)
            {
                foreach (var a in arduinos)
                {
                    id = a.Id;
                    plantName = a.Plant_name;
                }
            }
        }

        public async Task<Tuple<List<string>, List<decimal>, string>> loadData(string elementId, string sensorType, int descLimit)
        {
            Labels = new List<string>();
            Data = new List<decimal>();
            readings = new List<ReadingModel>();
            // Empty all data we currently have
            Labels.Clear();
            Data.Clear();
            readings.Clear();
            // Select the data we need from the database and pass the necesarry variables required to do so
            string sql = "select * from reading where arduino_id = @arduino_id and sensor_type = @sensor_type order by id desc limit @desc_limit";
            readings = await _data.LoadData<ReadingModel, dynamic>(sql, new { arduino_id = id, sensor_type = sensorType, desc_limit = descLimit }, ConnectionData.ConnectionString);
            // Go through all the received data and specifically grab/convert the data we need to pass to the graph
            foreach (var reading in readings)
            {
                Labels.Add(reading.Post_date.ToString("HH:mm"));
                Data.Add(reading.ReadingValue);
            }
            // Reverse the lists since the sql query required it to be Descending to get only the last results necesarry
            Labels.Reverse();
            Data.Reverse();
            // return data
            return Tuple.Create(Labels, Data, elementId);
        }

        public async Task<int> latestValue(string sensorType)
        {
            Data = new List<decimal>();
            readings = new List<ReadingModel>();
            // Empty all data we currently have
            Data.Clear();
            readings.Clear();
            // Select the data we need from the database and pass the necesarry variables required to do so
            string sql = "select * from reading where arduino_id = @arduino_id and sensor_type = @sensor_type order by id desc limit 1";
            readings = await _data.LoadData<ReadingModel, dynamic>(sql, new { arduino_id = id, sensor_type = sensorType }, ConnectionData.ConnectionString);
            // Go through all the received data and specifically grab/convert the data we need to pass to the graph
            foreach (var reading in readings)
            {
                Data.Add(reading.ReadingValue);
            }
            // Select the data we need from the database and pass the necesarry variables required to do so
            int value = Convert.ToInt32(Data[0]);
            return value;
        }

        public async Task<bool> changePlantName(string inputPlantName, int userID)
        {
            // Update plant name in database, save it in the variable and return succes
            string sql = "update arduino set plant_name = @plantName WHERE user_id = @sessionUserID;";
            await _data.SaveData(sql, new { plantName = inputPlantName, sessionUserID = userID }, ConnectionData.ConnectionString);
            plantName = inputPlantName;
            return true;
        }
    }
}
