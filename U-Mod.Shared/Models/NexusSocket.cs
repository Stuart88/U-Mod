using System;
using System.Threading;
using System.Threading.Tasks;
using U_Mod.Shared.Constants;
using U_Mod.Shared.Models.Nexus;
using Websocket.Client;

namespace U_Mod.Shared.Models
{
    public class NexusExceptionArgs : EventArgs
    {
        #region Public Properties

        public Exception ex { get; set; }

        #endregion Public Properties
    }

    public class NexusResponseEventArgs : EventArgs
    {
        #region Public Properties

        public string ApiKey { get; set; }
        public string ConnectionToken { get; set; }

        #endregion Public Properties
    }

    public class NexusSocket
    {
        #region Public Fields

        public readonly string ApplicationSlug = Constants.Constants.NexusAppSlug;

        #endregion Public Fields

        #region Public Properties

        public string ApiKey { get; set; }
        public string ConnectionToken { get; set; }
        public NexusRequestData data { get; set; }
        public NexusResponse Response { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        #endregion Public Properties

        #region Public Events

        public event EventHandler ExceptionHappened;

        public event EventHandler GotApiKey;

        public event EventHandler GotConnectionToken;

        #endregion Public Events

        #region Public Methods

        public void ConnectToSocket()
        {
            var exitEvent = new ManualResetEvent(false);
            var url = new Uri("wss://sso.nexusmods.com");

            using (var client = new WebsocketClient(url))
            {
                client.ReconnectTimeout = TimeSpan.FromSeconds(30);
                //client.ReconnectionHappened.Subscribe(info => Log.Information($"Reconnection happened, type: {info.Type}"));

                client.MessageReceived.Subscribe(msg =>
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine($"Message received: {msg}");

                        Response = System.Text.Json.JsonSerializer.Deserialize<NexusResponse>(msg.Text);

                        if (Response.success)
                        {
                            if (!string.IsNullOrEmpty(Response.data.connection_token))
                            {
                                ConnectionToken = Response.data.connection_token;

                                OnGotConnectionToken(new NexusResponseEventArgs
                                {
                                    ConnectionToken = ConnectionToken
                                });
                            }

                            if (!string.IsNullOrEmpty(Response.data.api_key))
                            {
                                ApiKey = Response.data.api_key;

                                OnGotApiKey(new NexusResponseEventArgs
                                {
                                    ApiKey = ApiKey
                                });
                            }

                            //Reset data so there's definitely nothing there (old data) if using Response object later.
                            Response.data = new NexusData();
                        }
                        else
                        {
                            throw new Exception(Response.error.ToString());
                        }
                    }
                    catch (Exception e)
                    {
                        OnException(new NexusExceptionArgs { ex = e });
                    }
                });

                client.MessageReceived.Subscribe(msg =>

                    Response = System.Text.Json.JsonSerializer.Deserialize<NexusResponse>(msg.Text)
                );

                client.Start();

                data = new NexusRequestData
                {
                    id = Uuid,
                    protocol = 2,
                    token = ConnectionToken
                };

                Task.Run(() =>
                {
                    client.Send(System.Text.Json.JsonSerializer.Serialize(data));
                });

                exitEvent.WaitOne();
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void OnException(NexusExceptionArgs e)
        {
            EventHandler handler = ExceptionHappened;
            handler?.Invoke(this, e);
        }

        protected virtual void OnGotApiKey(NexusResponseEventArgs e)
        {
            EventHandler handler = GotApiKey;
            handler?.Invoke(this, e);
        }

        protected virtual void OnGotConnectionToken(NexusResponseEventArgs e)
        {
            EventHandler handler = GotConnectionToken;
            handler?.Invoke(this, e);
        }

        #endregion Protected Methods
    }
}