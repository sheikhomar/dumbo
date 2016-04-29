using System;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace dumbo.WpfApp.Editor
{
    // Based on http://stackoverflow.com/questions/23186060/avalonedit-is-it-possible-to-highlight-this-syntax/23251990#23251990
    public class ColorizeAvalonEdit : DocumentColorizingTransformer
    {
        protected override void ColorizeLine(DocumentLine line)
        {
            int lineStartOffset = line.Offset;
            string text = CurrentContext.Document.GetText(line);
            int start = 0;
            int index;
            while ((index = text.IndexOf("end", start, StringComparison.Ordinal)) >= 0)
            {
                base.ChangeLinePart(
                    lineStartOffset + index, // startOffset
                    lineStartOffset + index + 10, // endOffset
                    (VisualLineElement element) =>
                    {
                        // This lambda gets called once for every VisualLineElement
                        // between the specified offsets.
                        Typeface tf = element.TextRunProperties.Typeface;
                        // Replace the typeface with a modified version of
                        // the same typeface
                        element.TextRunProperties.SetTypeface(new Typeface(
                            tf.FontFamily,
                            FontStyles.Italic,
                            FontWeights.Bold,
                            tf.Stretch
                            ));
                    });
                start = index + 1; // search for next occurrence
            }
        }
    }
}