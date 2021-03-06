﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AZMonitoring.DAL
{
    partial class DAL
    {
        internal async Task<string> AddGInstruct(GInstruct newGI)
        {
            try
            {
                newGI.ID = (await client.PushAsync(pathginstruct, newGI)).Result.name;
                UpdateID(pathginstruct + newGI.ID, newGI.ID);
                AddGInstructToProvince(newGI.IDProvince, newGI.ID);
                return newGI.ID;
            }
            catch (Exception ex) { MessageBox.Show($"حدث خطأ \nكود الخطأ\n{ex.Message}", "حطأ", MessageBoxButton.OK, MessageBoxImage.Error); return ""; }
        }
        internal async Task<bool> UpdateGInstruct(GInstruct newGI)
        {
            try
            {
                await client.UpdateAsync(pathadmin + newGI.ID, newGI);
                return true;
            }
            catch (Exception ex) { MessageBox.Show($"حدث خطأ \nكود الخطأ\n{ex.Message}", "حطأ", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
        }
        internal async Task<GInstruct> GetGInstructbyID(string ID)
        {
            try
            {
                return (await client.GetAsync(pathginstruct + ID)).ResultAs<GInstruct>();
            }
            catch (Exception ex) { MessageBox.Show($"حدث خطأ \nكود الخطأ\n{ex.Message}", "حطأ", MessageBoxButton.OK, MessageBoxImage.Error); return null; }
        }
        internal async Task<bool> DeleteGInstructbyID(string ID)
        {
            try
            {
                await client.DeleteAsync(pathginstruct + ID);
                return true;
            }
            catch (Exception ex) { MessageBox.Show($"حدث خطأ \nكود الخطأ\n{ex.Message}", "حطأ", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
        }
        internal async Task<List<GInstruct>> GetGInstructs(List<string> IDS)
        {
            try
            {
                if (IDS == null || IDS.Count == 0) { return null; }
                var ls = new List<GInstruct>();
                foreach (var item in IDS)
                {
                    ls.Add(await GetGInstructbyID(item));
                }
                return ls;
            }
            catch { return null; }
        }
        internal async void AddInstructToGInstruct(string GIID,string ID)
        {
            try
            {
                List<string> ls = (await client.GetAsync(pathginstruct + GIID + "/InstructsID")).ResultAs<List<string>>();
                if (ls == null) { ls = new List<string>(); }
                ls.Add(ID);
                await client.SetAsync(pathginstruct + GIID + "/InstructsID", ls);
            }
            catch { }
        }
    }
}
