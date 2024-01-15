#if LITMOTION_SUPPORT_UNIRX
using System;
using UniRx;

namespace LitMotion
{
    /// <summary>
    /// Provides extension methods for UniRx integration.
    /// </summary>
    public static class LitMotionUniRxExtensions
    {
        /// <summary>
        /// Create the motion as IObservable.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <returns>Observable of the created motion.</returns>
        public static IObservable<TValue> ToObservable<TValue, TOptions, TAdapter>(this MotionBuilder<TValue, TOptions, TAdapter> builder)
            where TValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
            var subject = new Subject<TValue>();
            var callbacks = builder.BuildCallbackData(subject, (x, subject) => subject.OnNext(x));
            callbacks.OnCompleteAction = builder.buffer.OnComplete;
            callbacks.OnCompleteAction += () => subject.OnCompleted();
            var scheduler = builder.buffer.Scheduler;
            var entity = builder.BuildMotionData();

            builder.Schedule(scheduler, ref entity, ref callbacks);
            return subject;
        }

        /// <summary>
        /// Create a motion data and bind it to ReactiveProperty.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="progress">Target object that implements IProgress</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToReactiveProperty<TValue, TOptions, TAdapter>(this MotionBuilder<TValue, TOptions, TAdapter> builder, ReactiveProperty<TValue> reactiveProperty)
            where TValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
            Error.IsNull(reactiveProperty);
            return builder.BindWithState(reactiveProperty, (x, target) =>
            {
                target.Value = x;
            });
        }
    }
}
#endif