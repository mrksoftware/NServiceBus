namespace NServiceBus.AcceptanceTests.Routing
{
    using System;
    using AcceptanceTesting;

    static class SubscriptionBehaviorExtensions
    {
        public static void OnEndpointSubscribed<TContext>(this EndpointConfiguration configuration, Action<SubscriptionEventArgs, TContext> action) where TContext : ScenarioContext
        {
            configuration.Pipeline.Register("NotifySubscriptionBehavior", builder =>
            {
                var context = builder.Build<TContext>();
                return new SubscriptionBehavior<TContext>(action, context, MessageIntentEnum.Subscribe);
            }, "Provides notifications when endpoints subscribe");
        }

        public static void OnEndpointUnsubscribed<TContext>(this EndpointConfiguration configuration, Action<SubscriptionEventArgs, TContext> action) where TContext : ScenarioContext
        {
            configuration.Pipeline.Register("NotifyUnsubscriptionBehavior", builder =>
            {
                var context = builder.Build<TContext>();
                return new SubscriptionBehavior<TContext>(action, context, MessageIntentEnum.Unsubscribe);
            }, "Provides notifications when endpoints unsubscribe");
        }
    }
}