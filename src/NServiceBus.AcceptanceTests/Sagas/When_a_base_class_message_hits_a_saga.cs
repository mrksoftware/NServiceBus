﻿namespace NServiceBus.AcceptanceTests.Sagas
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus.AcceptanceTesting;
    using NServiceBus.AcceptanceTests.EndpointTemplates;
    using NServiceBus.Saga;
    using NUnit.Framework;

    [TestFixture]
    public class When_a_base_class_message_hits_a_saga
    {
        [Test]
        public async Task Should_find_existing_instance()
        {
            var correlationId = Guid.NewGuid();
            var context = await Scenario.Define<Context>()
                   .WithEndpoint<SagaEndpoint>(b => b.Given(bus =>
                   {
                       bus.SendLocal(new StartSagaMessage
                       {
                           SomeId = correlationId
                       });
                       return Task.FromResult(0);
                   }))
                   .Done(c => c.SecondMessageFoundExistingSaga)
                   .Run(TimeSpan.FromSeconds(20));

            Assert.True(context.SecondMessageFoundExistingSaga);
        }

        public class Context : ScenarioContext
        {
            public bool SecondMessageFoundExistingSaga { get; set; }
        }

        public class SagaEndpoint : EndpointConfigurationBuilder
        {
            public SagaEndpoint()
            {
                EndpointSetup<DefaultServer>();
            }

            public class TestSaga04 : Saga<TestSaga04.SagaData04>, IAmStartedByMessages<StartSagaMessageBase>
            {
                public Context Context { get; set; }

                public void Handle(StartSagaMessageBase message)
                {
                    if (Data.SomeId != Guid.Empty)
                    {
                        Context.SecondMessageFoundExistingSaga = true;
                    }
                    else
                    {
                        Data.SomeId = message.SomeId;

                        Bus.SendLocal(new StartSagaMessage
                        {
                            SomeId = message.SomeId
                        });
                    }
                }

                protected override void ConfigureHowToFindSaga(SagaPropertyMapper<SagaData04> mapper)
                {
                    mapper.ConfigureMapping<StartSagaMessageBase>(m => m.SomeId)
                        .ToSaga(s => s.SomeId);
                }

                public class SagaData04 : ContainSagaData
                {
                    public virtual Guid SomeId { get; set; }
                }
            }

        }


        public class StartSagaMessage : StartSagaMessageBase
        {
        }
        public class StartSagaMessageBase : IMessage
        {
            public Guid SomeId { get; set; }
        }


    }
}