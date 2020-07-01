using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core
{
    public interface ICanvas : IData, IUid, IElementType, ISelected
    {
        Action<IElement> ElementAdded { get; set; }

        IEnumerable<IElement> GetElements();

        void Add(IElement element);
        void Remove(IElement element);
        void Clear();

        double GetWidth();
        void SetWidth(double width);
        double GetHeight();
        void SetHeight(double height);

        List<object> GetTags();
        void SetTags(List<object> tags);

        IEnumerable<IElement> HitTest(IPoint point, double radius);
        IEnumerable<IElement> HitTest(IRect rect);

        IdCounter GetCounter();
        void SetCounter(IdCounter counter);

        SchemeProperties GetProperties();
        void SetProperties(SchemeProperties properties);
    }
}
