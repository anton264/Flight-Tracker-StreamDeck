﻿using FlightStreamDeck.Logics.Actions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpDeck;
using SharpDeck.Events.Received;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlightStreamDeck.Logics
{
    public class DeckLogic
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly IServiceProvider serviceProvider;
        private StreamDeckClient client;

        public DeckLogic(ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            this.loggerFactory = loggerFactory;
            this.serviceProvider = serviceProvider;
        }

        public void Initialize()
        {
            var args = Environment.GetCommandLineArgs();
            loggerFactory.CreateLogger<DeckLogic>().LogInformation("Initialize with args: {args}", string.Join("|", args));

            client = new StreamDeckClient(args[1..], loggerFactory.CreateLogger<StreamDeckClient>());

            client.RegisterAction("tech.flighttracker.streamdeck.generic.toggle", () => (GenericToggleAction)ActivatorUtilities.CreateInstance(serviceProvider, typeof(GenericToggleAction)));
            client.RegisterAction("tech.flighttracker.streamdeck.master.activate", () => (ApToggleAction)ActivatorUtilities.CreateInstance(serviceProvider, typeof(ApToggleAction)));
            client.RegisterAction("tech.flighttracker.streamdeck.heading.activate", () => (ApToggleAction)ActivatorUtilities.CreateInstance(serviceProvider, typeof(ApToggleAction)));
            client.RegisterAction("tech.flighttracker.streamdeck.nav.activate", () => (ApToggleAction)ActivatorUtilities.CreateInstance(serviceProvider, typeof(ApToggleAction))); 
            client.RegisterAction("tech.flighttracker.streamdeck.approach.activate", () => (ApToggleAction)ActivatorUtilities.CreateInstance(serviceProvider, typeof(ApToggleAction)));
            client.RegisterAction("tech.flighttracker.streamdeck.altitude.activate", () => (ApToggleAction)ActivatorUtilities.CreateInstance(serviceProvider, typeof(ApToggleAction)));
            client.RegisterAction("tech.flighttracker.streamdeck.heading.increase", () => (ValueChangeAction)ActivatorUtilities.CreateInstance(serviceProvider, typeof(ValueChangeAction)));
            client.RegisterAction("tech.flighttracker.streamdeck.heading.decrease", () => (ValueChangeAction)ActivatorUtilities.CreateInstance(serviceProvider, typeof(ValueChangeAction)));
            client.RegisterAction("tech.flighttracker.streamdeck.altitude.increase", () => (ValueChangeAction)ActivatorUtilities.CreateInstance(serviceProvider, typeof(ValueChangeAction)));
            client.RegisterAction("tech.flighttracker.streamdeck.altitude.decrease", () => (ValueChangeAction)ActivatorUtilities.CreateInstance(serviceProvider, typeof(ValueChangeAction)));
            client.RegisterAction("tech.flighttracker.streamdeck.switch", () => (SwitchAction)ActivatorUtilities.CreateInstance(serviceProvider, typeof(SwitchAction)));

            client.RegisterAction("tech.flighttracker.streamdeck.number.enter", () => (EnterAction)ActivatorUtilities.CreateInstance(serviceProvider, typeof(EnterAction)));

            for (var i = 0; i <= 9; i++)
            {
                client.RegisterAction("tech.flighttracker.streamdeck.number." + i, () => (NumberAction)ActivatorUtilities.CreateInstance(serviceProvider, typeof(NumberAction)));
            }


            client.KeyDown += Client_KeyDown;
            
            Task.Run(() =>
            {
                client.Start(); // continuously listens until the connection closes
            });
        }

        private void Client_KeyDown(object sender, ActionEventArgs<KeyPayload> e)
        {
            //client.SetTitleAsync(e.Context, "Hello world");
        }
    }
}
