using Moq;
using NUnit.Framework;
using System;

namespace Sicon.CrossCutMessaging.UnitTest
{
	public class Tests
	{
		Sage.Common.Messaging.MessageService _messageService = null;
		Sage.Common.Messaging.CrossCutMessageSource POP_SAVED_SOURCE = null;
		Mock<Sage.Common.Messaging.MessageHandler> mockMessageHandler = null;

		object dummySender = null;

		[SetUp]
		public void Setup()
		{
			_messageService = new Sage.Common.Messaging.MessageService();

			mockMessageHandler = new Moq.Mock<Sage.Common.Messaging.MessageHandler>();
			mockMessageHandler.Setup(_ => _(It.IsAny<object>(), It.IsAny<Sage.Common.Messaging.MessageArgs>()))
							  .Returns(new Sage.Common.Messaging.Response(Sage.Common.Messaging.ResponseArgs.Empty));

			dummySender = new object();

			POP_SAVED_SOURCE = new Sage.Common.Messaging.CrossCutMessageSource("POPOrder", "Saved", Sage.Common.Messaging.ProcessPoint.PostMethod);
		}

		[Test]
		public void SameInstanceNotifiedTest()
		{
			_messageService.Subscribe(POP_SAVED_SOURCE, mockMessageHandler.Object);

			var args = new Sage.Common.Messaging.MessageArgs();
			var responses = _messageService.Notify(POP_SAVED_SOURCE, dummySender, args);

			mockMessageHandler.Verify(c => c(dummySender, args), Times.Once);
		}

		[Test]
		public void DifferentInstanceNotifiedTest()
		{
			var source = new Sage.Common.Messaging.CrossCutMessageSource("POPOrder", "Saved", Sage.Common.Messaging.ProcessPoint.PostMethod);

			_messageService.Subscribe(POP_SAVED_SOURCE, mockMessageHandler.Object);
			_messageService.Subscribe(source, mockMessageHandler.Object);

			var args = new Sage.Common.Messaging.MessageArgs();
			var responses = _messageService.Notify(POP_SAVED_SOURCE, dummySender, args);

			mockMessageHandler.Verify(c => c(dummySender, args), Times.Exactly(2));
		}

		[Test]
		public void DifferentCrossCutNameSourcesNotNotifiedTest()
		{
			var source = new Sage.Common.Messaging.CrossCutMessageSource("SOPOrder", "Saved", Sage.Common.Messaging.ProcessPoint.PostMethod);
			_messageService.Subscribe(source, mockMessageHandler.Object);

			var args = new Sage.Common.Messaging.MessageArgs();
			var responses = _messageService.Notify(POP_SAVED_SOURCE, dummySender, args);

			mockMessageHandler.Verify(c => c(dummySender, args), Times.Never);
		}

		[Test]
		public void DifferentElementNameSourcesNotNotifiedTest()
		{
			var source = new Sage.Common.Messaging.CrossCutMessageSource("POPOrder", "Deleted", Sage.Common.Messaging.ProcessPoint.PostMethod);
			_messageService.Subscribe(source, mockMessageHandler.Object);

			var args = new Sage.Common.Messaging.MessageArgs();
			var responses = _messageService.Notify(POP_SAVED_SOURCE, dummySender, args);

			mockMessageHandler.Verify(c => c(dummySender, args), Times.Never);
		}

		[Test]
		public void CaseSensitivityTest()
		{
			var source = new Sage.Common.Messaging.CrossCutMessageSource("poporder", "saved", Sage.Common.Messaging.ProcessPoint.PostMethod);
			_messageService.Subscribe(source, mockMessageHandler.Object);

			var args = new Sage.Common.Messaging.MessageArgs();
			var responses = _messageService.Notify(POP_SAVED_SOURCE, dummySender, args);

			mockMessageHandler.Verify(c => c(dummySender, args), Times.Once);
		}
	}
}