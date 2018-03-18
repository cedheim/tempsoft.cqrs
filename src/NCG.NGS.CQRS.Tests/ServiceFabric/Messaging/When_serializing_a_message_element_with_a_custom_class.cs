﻿using System.IO;
using System.Runtime.Serialization;
using System.Text;
using FluentAssertions;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Messaging;
using NCG.NGS.CQRS.Tests.Mocks;
using NUnit.Framework;

namespace NCG.NGS.CQRS.Tests.ServiceFabric.Messaging
{
    [TestFixture]
    public class When_serializing_a_message_element_with_a_custom_class
    {
        private MessageBody _input;
        private MessageBody _output;
        private string _xml;


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _input = new MessageBody(new ChangedAValue(5));
            var serializer = new DataContractSerializer(typeof(MessageBody));

            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, _input);
                stream.Flush();

                _xml = Encoding.UTF8.GetString(stream.ToArray());

                stream.Seek(0, SeekOrigin.Begin);

                _output = serializer.ReadObject(stream) as MessageBody;
            }
        }

        [Test]
        public void Should_have_resulted_in_the_same_object()
        {
            _output.Should().BeEquivalentTo(_input);
        }
    }
}