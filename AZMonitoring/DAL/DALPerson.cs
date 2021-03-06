﻿using Firebase.Auth;
using Firebase.Storage;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AZMonitoring.DAL
{
    partial class DAL
    {
        EventStreamResponse personlisner;
        internal async Task<bool> IsOnline(string id)
        {
            try
            {
                return (await client.GetAsync(pathperson + id + "/Online")).ResultAs<string>() == "true" ? true : false;
            }
            catch { return false; }
        }
        internal async Task<bool> AddPerson(Person newPerson)
        {
            try
            {
                await client.SetAsync(pathperson + newPerson.ID, newPerson);
                return true;
            }
            catch (Exception ex) { MessageBox.Show($"حدث خطأ \nكود الخطأ\n{ex.Message}", "حطأ", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
        }
        internal async Task<bool> UpdatePerson(Person person)
        {
            try
            {
                await client.UpdateAsync(pathperson + person.ID, person);
                return true;
            }
            catch (Exception ex) { MessageBox.Show($"حدث خطأ \nكود الخطأ\n{ex.Message}", "حطأ", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
        }
        internal async Task<bool> UpdatePersonPosition(string id,string personposition)
        {
            try
            {
                await client.SetAsync(pathperson + id + "/IDPosition/", personposition);
                return true;
            }
            catch (Exception ex) { MessageBox.Show($"حدث خطأ في تعديل وظيفة شخص\nكود الخطأ\n{ex.Message}", "حطأ", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
        }
        internal async Task<string> GetPositionID(string PersonID)
        {
            try
            {
                var snap = await client.GetAsync(pathperson + PersonID + "/IDPosition");
                return snap.ResultAs<string>();
            }
            catch (Exception ex) { MessageBox.Show($"حدث خطأ \nكود الخطأ\n{ex.Message}", "حطأ", MessageBoxButton.OK, MessageBoxImage.Error); return null; }
        }
        internal async Task<Person> GetLogedPerson(string PersonID,string password)
        {
            try
            {
                var snap = await client.GetAsync(pathperson +  PersonID + "/Password");
                if(password == snap.ResultAs<string>())
                {
                    return (await client.GetAsync(pathperson + PersonID)).ResultAs<Person>();
                }
                return null;
            }
            catch (Exception ex) { MessageBox.Show($"حدث خطأ \nكود الخطأ\n{ex.Message}", "حطأ", MessageBoxButton.OK, MessageBoxImage.Error); return null; }
        }
        internal async Task<Person> GetPersonbyID(string PersonID)
        {
            try
            {
                var snap = await client.GetAsync(pathperson + PersonID);
                var p = snap.ResultAs<Person>();
                p.Password = p.SSN = "";
                p.Chats = null;
                return p;
            }
            catch (Exception ex) { MessageBox.Show($"حدث خطأ \nكود الخطأ\n{ex.Message}", "حطأ", MessageBoxButton.OK, MessageBoxImage.Error); return null; }
        }
        internal async Task<bool> AvailableID(string PersonID)
        {
            try
            {
                var snap = await client.GetAsync(pathperson + PersonID);
                var p = snap.ResultAs<Person>();
                if (p == null) { return true; }
                return false;
            }
            catch (Exception ex) { return false; }
        }
        internal async Task<bool> AddChatToPerson(string PersonID, string chatid)
        {
            try
            {
                var snap = await client.GetAsync(pathperson + PersonID + "/ChatsID");
                var chts = snap.ResultAs<List<string>>();
                chts.Add(chatid);
                await client.UpdateAsync(pathperson + PersonID + "/ChatsID", chts);
                return true;
            }
            catch (Exception ex) { MessageBox.Show($"حدث خطأ \nكود الخطأ\n{ex.Message}", "حطأ", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
        }
        internal async Task<string> AddPersonImage(string PersonID, FileStream img)
        {
            try
            {
                var auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyA4FzwRW0vN6Q6IEVJf_GFOAre0uj4zr44"));
                var a = await auth.SignInAnonymouslyAsync();
                return await statics.UploadImage(PersonID, img, "fir-test1-fb35d.appspot.com", a);
            }
            catch (Exception ex) { MessageBox.Show($"حدث خطأ \nكود الخطأ\n{ex.Message}", "حطأ", MessageBoxButton.OK, MessageBoxImage.Error); return ""; }
        }
        internal async Task<Person> GetPersonbyPositionID(string PositionID)
        {
            try
            {
                return await GetPersonbyID(await GetPositionPersonIDByID(PositionID));
            }
            catch (Exception ex) { MessageBox.Show($"حدث خطأ \nكود الخطأ\n{ex.Message}", "حطأ", MessageBoxButton.OK, MessageBoxImage.Error); return null; }
        }
        internal async Task<bool> DeletePersonbyID(string ID)
        {
            try
            {
                await client.DeleteAsync(pathperson + ID);
                return true;
            }
            catch (Exception ex) { MessageBox.Show($"حدث خطأ \nكود الخطأ\n{ex.Message}", "حطأ", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
        }
        internal async Task<string> GetPersonPhoto(string id)
        {
            try
            {
                var x = (await client.GetAsync(pathperson + id + "/Photo")).ResultAs<string>();
                return x;
            }
            catch { return ""; }
        }
        internal async void AddChatstoPerson(Chats chat , string id)
        {
            try
            {
                var p = (await client.GetAsync(pathperson + id + "/Chats/")).ResultAs<List<Chats>>();
                if(p == null || p.Count < 1) { p = new List<Chats>(); }
                p.Add(chat);
                await client.SetAsync(pathperson + id + "/Chats/", p);
            }
            catch {  }
        }
        internal async Task<string> GetPersonName(string id)
        {
            try
            {
                return (await client.GetAsync(pathperson + id + "/Name")).ResultAs<string>();
            }
            catch { return ""; }
        }
        internal async Task<string> GetCurrentStream(string personid)
        {
            try
            {
                return (await client.GetAsync(pathperson + personid + "/CurrentStream")).ResultAs<string>();
            }
            catch { return ""; }
        }
        internal async Task<bool> CheckPersonStreamAvailable(string personid)
        {
            try
            {
                string x = (await GetCurrentStream(personid));
                return x == null ? true : (x.Replace(" ", "") == "" || x.Replace(" ", "")  == "active" ? true:false);
            }
            catch { return false; }
        }
        internal async void SetVideoChat(string partnerid,OpenTokConfig config)
        {
            try
            {
                await client.SetAsync(pathperson + statics.LogedPerson.ID + "/CurrentStream", $"InCall");
                await client.SetAsync(pathperson + partnerid + "/CurrentStream", "ReceivingCall");
                await client.SetAsync(pathperson + partnerid + "/OpenTokConfig", config);
            }
            catch(Exception ex) { }
        }
        internal async void CloseComingVideoChat(string partnerid)
        {
            try
            {
                await client.SetAsync(pathperson + statics.LogedPerson.ID + "/CurrentStream", "active");
                await client.SetAsync(pathperson + partnerid + "/CurrentStream", "active");
                await client.DeleteAsync(pathperson + statics.LogedPerson.ID + "/OpenTokConfig");

            }
            catch { }
        }
        internal async void CloseVideoChat(string partnerid)
        {
            try
            {
                await client.SetAsync(pathperson + statics.LogedPerson.ID + "/CurrentStream", "active");
                await client.SetAsync(pathperson + partnerid + "/CurrentStream", "active");

            }
            catch { }
        }
        internal async void SetStreamListener(string id,GetChatStream @delegate)
        {
            try
            {
                personlisner = await client.OnAsync($"{pathperson}{id}/CurrentStream", changed: async (ck,snap,sad)=> {
                    if (snap.Data == "ReceivingCall")
                    {
                        @delegate.Invoke(await GetStreamConfig(id));
                    }
                });
            }
            catch { }
        }
        private async Task<OpenTokConfig> GetStreamConfig(string id)
        {
            try
            {
                return (await client.GetAsync(pathperson + id + "/OpenTokConfig")).ResultAs<OpenTokConfig>();
            }
            catch { return null; }
        }
        internal void DisposeStreamListener()
        {
            personlisner?.Dispose();
        }
    }
}
