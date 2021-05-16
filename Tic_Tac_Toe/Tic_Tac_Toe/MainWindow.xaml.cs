//Marco Casali 4L 16/05/2021
//Tic-Tac-Toe con connessione TCP

using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;

namespace Tic_Tac_Toe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeGrid();
            lblStatus.HorizontalContentAlignment = HorizontalAlignment.Center;
            lblChar.HorizontalContentAlignment = HorizontalAlignment.Right;
        }

        private Button[,] buttons;
        private bool _isHost = false;
        private string rematchFlags = ""; 
        private TcpListener server = null;
        private TcpClient client = null;
        private Socket socket = null;

        /// <summary>
        /// Creazione griglia di gioco
        /// </summary>
        private void InitializeGrid()
        {
            buttons = new Button[3, 3];
            //Reset griglia
            grdGame.ColumnDefinitions.Clear();
            grdGame.RowDefinitions.Clear();
            grdGame.Children.Clear();
            //Creazione righe
            for (int r = 0; r < 3; r++)
            {
                grdGame.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) });
            }
            //Creazione colonne
            for (int c = 0; c < 3; c++)
            {
                grdGame.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Star) });
            }
            //Setup dei bottoni
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    buttons[r, c] = new Button()
                    {
                        Tag = r.ToString() + "," + c.ToString(),
                        FontSize = 46,
                        Content = " ",
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(0.6),
                        IsEnabled = false
                    };
                    buttons[r, c].Click += Grid_Button_Click; //Evento click
                    Grid.SetRow(buttons[r, c], r);
                    Grid.SetColumn(buttons[r, c], c);
                    grdGame.Children.Add(buttons[r, c]);
                }
            }
        }

        /// <summary>
        /// Evento click casella
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_Button_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            if (_isHost)
                b.Content = "X";
            else
                b.Content = "O";

            ButtonsOFF();
            SendData(b.Tag.ToString()); //Manda mossa all'avversario
            if (CheckWin()) //Controllo vincita
            {
                UpdateGameStatus("You Won!");
                btnRematch.IsEnabled = true;
            }
            else if(CheckDraw()) //Controllo pareggio
            {
                UpdateGameStatus("Draw!");
                btnRematch.IsEnabled = true;
            }
            else
            {
                UpdateGameStatus("Opponent's turn");
            }
        }

        /// <summary>
        /// Manda una stringa all'avversario
        /// </summary>
        /// <param name="message">Dati da inviare</param>
        private void SendData(string message)
        {
            byte[] bytesToSend = Encoding.ASCII.GetBytes(message);
            socket.Send(bytesToSend);
        }

        /// <summary>
        /// Setup del gioco parte server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHost_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int sourcePort = int.Parse(txtSourcePort.Text);
                if (sourcePort < 0 || sourcePort > 65535)
                    throw new Exception("Numero di porta non esistente.");

                _isHost = true;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, sourcePort);
                btnHost.IsEnabled = false;
                btnConnect.IsEnabled = false;
                lblChar.Content = "Your char: X";
                Server(); //Inizia il thread principale di ascolto
            }
            catch(Exception ex)
            {
                MessageBox.Show("Errore: \n" + ex.Message,"ERRORE",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Thread principale di ascolto
        /// </summary>
        private async void Server()
        {           
            await Task.Run(() => 
            {
                byte[] bytes = new byte[16];
                int i = 0;
                server.Start();
                UpdateStatus("Waiting for a connection...");
                socket = server.AcceptSocket();
                UpdateStatus("Connected to " + socket.RemoteEndPoint.ToString());
                ButtonsON();
                UpdateGameStatus("Your turn");
                while (socket != null) //Ascolta fino al termine della connessione
                {                   
                    string data = null;
                    try
                    {
                        if ((i = socket.Receive(bytes)) != 0) //Elabora i byte ricevuti
                        {
                            data = Encoding.ASCII.GetString(bytes, 0, i);
                            if (data == "abort")
                            {
                                Abort();
                                UpdateStatus("Connection terminated by opponent");
                            }
                            else if (data == "rematch")
                            {
                                UpdateStatus("Opponent wants to rematch");
                                rematchFlags += "1";
                                CheckRematch();
                            }
                            else
                            {
                                OpponentMove(int.Parse(data.Split(',')[0]), int.Parse(data.Split(',')[1]));
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }       
                }
            });
        }

        /// <summary>
        /// Aggiorna lo stato della connessione
        /// </summary>
        /// <param name="status">Messaggio</param>
        private void UpdateStatus(string status)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                txtblcStatus.Text = status;
            }));
        }

        /// <summary>
        /// Aggiorna lo stato del gioco
        /// </summary>
        /// <param name="status">Messaggio</param>
        private void UpdateGameStatus(string status)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                lblStatus.Content = status;
            }));
        }

        /// <summary>
        /// Attiva tutti i bottoni non utilizzati
        /// </summary>
        private void ButtonsON()
        {           
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                for (int r = 0; r < 3; r++)
                {
                    for (int c = 0; c < 3; c++)
                    {           
                        if (buttons[r, c].Content.ToString() == " ")
                            buttons[r, c].IsEnabled = true;
                    }
                }      
            }));
        }

        /// <summary>
        /// Disattiva tutti i bottoni
        /// </summary>
        private void ButtonsOFF()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                for (int r = 0; r < 3; r++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                         buttons[r, c].IsEnabled = false;
                    }
                }
            }));
        }

        /// <summary>
        /// Gestisce la mossa dell'avversario
        /// </summary>
        /// <param name="r">Posizione y del bottone</param>
        /// <param name="c">Posizione x del bottone</param>
        private void OpponentMove(int r, int c)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_isHost)
                    buttons[r, c].Content = "O";
                else
                    buttons[r, c].Content = "X";

                buttons[r, c].IsEnabled = false;

                if (CheckWin())
                {
                    UpdateGameStatus("You Lost!");
                    btnRematch.IsEnabled = true;
                }                 
                else if (CheckDraw())
                {
                    UpdateGameStatus("Draw!");
                    btnRematch.IsEnabled = true;
                }
                else
                {
                    ButtonsON();
                    UpdateGameStatus("Your Turn");
                }
            }));          
        }

        /// <summary>
        /// Setup del gioco lato client
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(txtDestinationIP.Text);
                int port = int.Parse(txtDestinationPort.Text);
                if (port < 0 || port > 65535)
                    throw new Exception("Porta non esistente.");

                _isHost = false;
                btnHost.IsEnabled = false;
                btnConnect.IsEnabled = false;
                lblChar.Content = "Your char: O";
                Client(ip, port); //Inizio thread principale client
            }
            catch(Exception ex)
            {
                MessageBox.Show("Errore: \n" + ex.Message, "ERRORE", MessageBoxButton.OK, MessageBoxImage.Error);
            }           
        }

        /// <summary>
        /// Thread principale client
        /// </summary>
        /// <param name="ip">IP del server</param>
        /// <param name="port">Port del server</param>
        private async void Client(object ip, object port)
        {
            await Task.Run(() =>
            {
                byte[] bytes = new byte[16];
                int i;
                UpdateStatus("Connecting to " + ip + ":" + port);
                client = new TcpClient(ip.ToString(), (int)port); //Crea la connessione
                socket = client.Client; //Estrae il socket
                UpdateStatus("Connected to " + socket.RemoteEndPoint.ToString());
                ButtonsOFF();
                UpdateGameStatus("Opponent's turn");
                while (socket != null) //Riceve fino alla fine della connessione
                {                 
                    string data = null;
                    try
                    {
                        if ((i = socket.Receive(bytes)) != 0)
                        {
                            data = Encoding.ASCII.GetString(bytes, 0, i); //Elabora i dati ricevuti
                            if (data == "abort")
                            {
                                Abort();
                                UpdateStatus("Connection terminated by opponent");
                            }
                            else if(data == "rematch")
                            {
                                UpdateStatus("Opponent wants to rematch");
                                rematchFlags += "1";
                                CheckRematch();
                            }
                            else
                            {
                                OpponentMove(int.Parse(data.Split(',')[0]), int.Parse(data.Split(',')[1]));
                            }

                        }
                    }
                    catch
                    {
                        continue;
                    }
                    
                }
            });
        }

        /// <summary>
        /// Gestisce il termine della connessione e reset dell'interfaccia
        /// </summary>
        private void Abort()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        buttons[i, j].Content = " ";
                    }
                }

                btnHost.IsEnabled = true;
                btnConnect.IsEnabled = true;
                btnRematch.IsEnabled = false;
                lblChar.Content = "";
            }));
            
            if (server != null)
            {
                server.Stop();
                server = null;
            }               
            if (client != null)
            {
                client.Close();
                client = null;
            }               
            if (socket != null)
            {
                socket.Close();
                socket = null;
            }
               
            ButtonsOFF();
            UpdateGameStatus("");
        }

        /// <summary>
        /// Controllo vittoria
        /// </summary>
        /// <returns>Esito controllo</returns>
        private bool CheckWin()
        {
            bool won = false;
            //righe
            for (int r = 0; r < 3 && won == false;r++)
            {
                won = true;
                string sign = buttons[r, 0].Content.ToString();
                for (int c = 0; c < 3 && won == true; c++)
                    if (sign != buttons[r, c].Content.ToString() || sign == " ")
                        won = false;
            }

            if (won) return true;

            //colonne
            for (int c = 0; c < 3 && won == false; c++)
            {
                won = true;
                string sign = buttons[0, c].Content.ToString();
                for (int r = 0; r < 3 && won == true; r++)
                    if (sign != buttons[r, c].Content.ToString() || sign == " ")
                        won = false;
            }

            if (won) return true;

            //diagonali
            if (buttons[0, 0].Content.ToString() == buttons[1, 1].Content.ToString() && buttons[1, 1].Content.ToString() == buttons[2, 2].Content.ToString() && buttons[2, 2].Content.ToString() != " ")
                return true;

            if (buttons[0, 2].Content.ToString() == buttons[1, 1].Content.ToString() && buttons[1, 1].Content.ToString() == buttons[2, 0].Content.ToString() && buttons[2, 0].Content.ToString() != " ")
                return true;

            return false;
        }

        /// <summary>
        /// Controllo pareggio
        /// </summary>
        /// <returns>Esito controllo</returns>
        private bool CheckDraw()
        {
            for(int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (buttons[i, j].Content.ToString() == " ")
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Avvisa l'avversario del termine
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            btnReset_Click(new object(), new RoutedEventArgs());
        }

        /// <summary>
        /// Termine della connessione
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            if(socket != null)
                SendData("abort");
            Abort();
            UpdateStatus("Connection terminated");
        }

        /// <summary>
        /// Inizio nuova partita con lo stesso avversario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRematch_Click(object sender, RoutedEventArgs e)
        {          
            rematchFlags += "1";
            CheckRematch();
            SendData("rematch");
            UpdateStatus("Waiting for opponent...");
        }

        /// <summary>
        /// Controlla se entrambi i giocatori intendono iniziare
        /// </summary>
        private void CheckRematch()
        {
            this.Dispatcher.BeginInvoke(new Action(()=> { 
                if(rematchFlags.Contains("11"))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            buttons[i, j].Content = " ";
                        }
                    }

                    UpdateStatus("Connected to " + socket.RemoteEndPoint.ToString());
                    if(_isHost)
                    {
                        UpdateGameStatus("Your Turn");
                        ButtonsON();
                    }
                    else
                    {
                        UpdateGameStatus("Opponent's Turn");
                        ButtonsOFF();
                    }

                    rematchFlags = "";
                    btnRematch.IsEnabled = false;
                }
            }));
        }
    }
}
