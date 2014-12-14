﻿// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using BusConfigurators;
    using Magnum.Extensions;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;

    public class When_a_message_is_published_to_a_subclass :
        Given_a_rabbitmq_bus
    {
        Future<B> _receivedB;

        protected override void ConfigureServiceBus(Uri uri, ServiceBusConfigurator configurator)
        {
            base.ConfigureServiceBus(uri, configurator);

            _receivedB = new Future<B>();

//            configurator.Subscribe(s => s.Handler<B>(async message => _receivedB.Complete(message.Message)));
        }

        [SetUp]
        public void A_message_is_published()
        {
            LocalBus.Publish(new A
                {
                    StringA = "ValueA",
                    StringB = "ValueB",
                });
        }

        [Test]
        public void Should_receive_the_inherited_version()
        {
            _receivedB.WaitUntilCompleted(8.Seconds()).ShouldBe(true);
            _receivedB.Value.StringB.ShouldBe("ValueB");
        }

        class A :
            B
        {
            public string StringA { get; set; }
        }

        class B
        {
            public string StringB { get; set; }
        }
    }

    
    public class When_a_message_is_published_and_nobody_is_listening :
        Given_a_rabbitmq_bus
    {
        [SetUp]
        public void A_message_is_published()
        {
            LocalBus.Publish(new Nobody_listening_parent
                {
                    StringA = "ValueA",
                    StringB = "ValueB",
                });
        }

        [Test]
        public void Should_receive_the_inherited_version()
        {
        }

        class Nobody_listening_child 
        {
            public string StringA { get; set; }
        }

        class Nobody_listening_parent : 
            Nobody_listening_child
        {
            public string StringB { get; set; }
        }
    }
}