using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;

namespace dumbo.WpfApp.Editor
{
    // Stolen from http://stackoverflow.com/questions/5072761/avalonedit-highlight-current-line-even-when-not-focused
    public class HighlightCurrentLineBackgroundRenderer : IBackgroundRenderer
    {
        private readonly TextEditor _editor;

        public HighlightCurrentLineBackgroundRenderer(TextEditor editor)
        {
            _editor = editor;
            _editor.TextArea.Caret.PositionChanged += (sender, e) =>
                _editor.TextArea.TextView.InvalidateLayer(KnownLayer.Background);
        }

        public KnownLayer Layer
        {
            get { return KnownLayer.Caret; }
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (_editor.Document == null)
                return;

            textView.EnsureVisualLines();
            var currentLine = _editor.Document.GetLineByOffset(_editor.CaretOffset);
            foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, currentLine))
            {
                var rect2 = new Rect(new Point(rect.Location.X + textView.ScrollOffset.X, rect.Location.Y), new Size(textView.ActualWidth, rect.Height));
                drawingContext.DrawRectangle(
                    new SolidColorBrush(Color.FromArgb(0x10, 0, 0, 0xFF)), null,
                    rect2);
            }
        }
    }
}