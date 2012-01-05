using System;
using System.Windows.Forms;
using Keyboard.PInvoke.SendInput.Native;

namespace Keyboard.PInvoke.SendInput
{
    public class SendInputKeyboard : IKeyboard
    {
        private readonly IInputDispatcher _dispatcher;

        public SendInputKeyboard(IInputDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        #region Implementation of IKeyboard

        public void KeyDown(Keys key)
        {
            INPUT[] input = new InputBuilder()
                .AddKeyDown(key)
                .BuildArray();

            _dispatcher.DispatchInput(input);
        }

        public void KeyUp(Keys key)
        {
            INPUT[] input = new InputBuilder()
                .AddKeyUp(key)
                .BuildArray();

            _dispatcher.DispatchInput(input);
        }

        public void KeyPress(Keys key)
        {
            INPUT[] input = BuildInputInModifierFrame(new ModifierExtractor(key), builder =>
            {
                builder.AddKeyDown(key);
                builder.AddKeyUp(key);
            });

            _dispatcher.DispatchInput(input);
        }

        private static INPUT[] BuildInputInModifierFrame(ModifierExtractor modifiers, Action<InputBuilder> action)
        {
            var builder = new InputBuilder();

            foreach (Keys modifier in modifiers.AsEnumerable())
                builder.AddKeyDown(modifier);

            action(builder);

            foreach (Keys modifier in modifiers.AsEnumerable())
                builder.AddKeyUp(modifier);

            return builder.BuildArray();
        }

        #endregion
    }
}