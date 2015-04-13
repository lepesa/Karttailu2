/*
   Copyright 2015 Esa Leppänen

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
namespace Karttailu2
{
    /// <summary>
    /// Simppeli loggeriuokka, joka pistää rivin käyttöliittymän textboxiin. 
    /// Tarkoitus vain alkuvaiheessa tarjota liittymä, jota käyttää.
    /// </summary>
    class Logger : ILog
    {
        private TextBox tb;

        // UI-elementti talteen muodostajassa
        public Logger(TextBox _tb)
        {
            tb = _tb;
        }

        /// <summary>
        ///  Kirjoitetaan tekstirivi UI-elementtiin.
        /// </summary>
        /// <param name="text">Tekstirivi</param>
        public void WriteLog(string text)
        {
            Application.Current.Dispatcher.BeginInvoke( DispatcherPriority.Background,
                new Action(() => tb.Text += text + Environment.NewLine));
        }
    }
}
