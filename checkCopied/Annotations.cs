using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace checkCopied
{
    internal class Annotations
    {
        public enum AnotationType
        {
            CUMPLE_RUTAS,
            CUMPLE_STORAGE,
            CUMPLE_CONTEXT
        }

        public class Annotation
        {
            public AnotationType Type { get; set; }
            public string Description { get; set; } = "";
        }

        public static class AnnotationDictionary
        {
            public static readonly Dictionary<AnotationType, string> Annotations = new Dictionary<AnotationType, string>()
            {
                { AnotationType.CUMPLE_RUTAS, "Has cumplido con las rutas requeridas." },
                { AnotationType.CUMPLE_STORAGE, "Has cumplido con las especificaciones del sotrage." },
                { AnotationType.CUMPLE_CONTEXT, "Has cumplido con las especificaciones del contexto." }
            };
        }
    }
}
