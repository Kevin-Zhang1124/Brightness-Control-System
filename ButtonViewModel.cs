using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;
using System.Text.Json;
using System.Diagnostics;


namespace StatusSwitchButton
{
    public class ButtonViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private enum ButtonState { State1 = 0, State2 = 30, State3 = 60 }
        private ButtonState _currentState;

        public string ButtonText => $"亮度： {((int)_currentState)}";

        public ICommand ToggleStateCommand { get; }

        public ButtonViewModel()
        {
            ToggleStateCommand = new RelayCommand(ToggleState);
            LoadState();
        }

        private void ToggleState()
        {
            switch (_currentState)
            {
                case ButtonState.State1:
                    _currentState = ButtonState.State2;
                    break;
                case ButtonState.State2:
                    _currentState = ButtonState.State3;
                    break;
                case ButtonState.State3:
                    _currentState = ButtonState.State1;
                    break;
                // 其他状态
                default:
                    _currentState = ButtonState.State1;
                    break;
            }
            OnPropertyChanged(nameof(ButtonText));
            ApplyBrightness((int)_currentState);
            SaveState();
        }

        // 设置新的亮度值 (set new brightness value)
        private void ApplyBrightness(int value) // 耗时5000+ms
        {
            try
            {
                BrightnessControl.SetBrightness(value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"亮度设置失败: {ex.Message}");
                // 这里可以添加用户提示逻辑
            }
        }

        // 定义文件路径常量 (define file path)
        private const string StateFilePath = "buttonState.json";

        // 保存每次修改后的状态 (save the status after each modification)
        private void SaveState()
        {
            var state = new { State = _currentState };
            var json = JsonSerializer.Serialize(state);
            File.WriteAllText(StateFilePath, json);
        }

        private void LoadState()
        {
            if (File.Exists(StateFilePath))
            {
                try
                {
                    var json = File.ReadAllText(StateFilePath);
                    var state = JsonSerializer.Deserialize<ButtonStateWrapper>(json);
                    _currentState = state?.State ?? ButtonState.State1;
                }
                catch (JsonException)
                {
                    _currentState = ButtonState.State1;
                }
            }
            else
            {
                _currentState = ButtonState.State1;
            }
        }

        private class ButtonStateWrapper
        {
            public ButtonState State { get; set; }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;

        public RelayCommand(Action execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _execute();

        public event EventHandler CanExecuteChanged;
    }

}
