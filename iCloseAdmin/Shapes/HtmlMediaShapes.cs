using OrchardCore.DisplayManagement.Descriptors;

namespace iCloseAdmin.Shapes
{
    public class HtmlMediaShapes : IShapeTableProvider
    {
        public void Discover(ShapeTableBuilder builder)
        {
            builder.Describe("HtmlBodyPart_Edit")
                .OnDisplaying(displaying =>
                {
                    var editor = displaying.Shape;

                    editor.Metadata.Wrappers.Add("Media_Wrapper__HtmlBodyPart");
                });
        }
    }
}
