using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace checkCopied
{
    internal class Annotations
    {
        public enum AnnotationType
        {
            E1_FUNCTIONALITY,
            E1_FOLDER_STRUCTURE,
            E1_COMPONENTS_STRUCTURE,
            E1_COMPONENTS_PROPS,
            E1_STATE_TO_CONTROL_CARD_VISIBILITY,
            E1_CARD_PROPS,
            E1_VALIDATION_FIRST_INPUT,
            E1_VALIDATION_SECOND_INPUT,
            E1_CONTROLLED_COMPONENTS,
            E1_FORM_SUBMIT,
            E2_USE_REDUCER,
            E2_INFINITE_LOOPS,
            E2_THEME_FUNCTIONALITY,
            E2_ROUTES,
            E2_ADD_FAVS,
            E2_KEY_ON_LIST,
            E2_DETAIL_FETCH,
            E2_FAVS_FUNCTIONALITY,
            E2_CONTACT_FORM_FUNCTIONALITY,
            //CUSTOM ANNOTATIONS
            NO_LEVANTA,
            LIBRERIAS_NO_INSTALADAS,
            ERROR_IMPORTAR_EXPORTAR,

        }

        public static class AnnotationDictionary
        {
            public static readonly Dictionary<AnnotationType, string> Annotations = new Dictionary<AnnotationType, string>()
            {
                { AnnotationType.E1_FUNCTIONALITY, "Funcionamiento de la aplicacion." },
                { AnnotationType.E1_FOLDER_STRUCTURE, "Estructura de carpetas." },
                { AnnotationType.E1_COMPONENTS_STRUCTURE, "Componentes bien importados" },
                { AnnotationType.E1_COMPONENTS_PROPS, "Los componentes reciben bien las props" },
                { AnnotationType.E1_STATE_TO_CONTROL_CARD_VISIBILITY, "State para controlar la visibilidad de las cards" },
                { AnnotationType.E1_CARD_PROPS, "Card recibe la info correcta" },
                { AnnotationType.E1_VALIDATION_FIRST_INPUT, "Validacion del primer input" },
                { AnnotationType.E1_VALIDATION_SECOND_INPUT, "Validacion del segundo input" },
                { AnnotationType.E1_CONTROLLED_COMPONENTS, "Inputs controlados correctamente" },
                { AnnotationType.E1_FORM_SUBMIT, "Submit manejado correctamente" },
                { AnnotationType.E2_USE_REDUCER, "manejo correcto de la informacion del context" },
                { AnnotationType.E2_INFINITE_LOOPS, "No hay bucles infinitos y se obtienen los datos a demanda" },
                { AnnotationType.E2_THEME_FUNCTIONALITY, "Se puede cambiar de tema y se mantiene al navegar" },
                { AnnotationType.E2_ROUTES, "Funcionan las 4 rutas solicitadas" },
                { AnnotationType.E2_ADD_FAVS, "Añadir a favoritos desde app" },
                { AnnotationType.E2_KEY_ON_LIST, "Uso de key correcto en listas" },
                { AnnotationType.E2_DETAIL_FETCH, "Se hace un nuevo fetch en el detalle" },
                { AnnotationType.E2_FAVS_FUNCTIONALITY, "Funcionalidad de favoritos correcta" },
                { AnnotationType.E2_CONTACT_FORM_FUNCTIONALITY, "Funcionalidad de formulario de contacto correcto" },

                { AnnotationType.NO_LEVANTA, "El proyecto no levanta" },
                { AnnotationType.ERROR_IMPORTAR_EXPORTAR, "Error al importar o exportar. Componentes indefinidos" },
                { AnnotationType.LIBRERIAS_NO_INSTALADAS, "Uso de librerias que no figuran en el package.json" }
            };
        }

        public class Annotation
        {
            public AnnotationType Type { get; set; }
            public bool IsCorrect { get; set; } = false;
            public string Description { get; set; } = "";
        }

        public class StudentAnnotations
        {
            public string StudentName { get; set; } = "";
            public List<Annotation> Annotations { get; set; } = new List<Annotation>();
        }

        public static StudentAnnotations[] marks = new StudentAnnotations[]
        {
            new StudentAnnotations() {
                StudentName = "Abril Guzman",
                Annotations = new List<Annotation>() {
                    new Annotation() {
                        Type = AnnotationType.E2_USE_REDUCER,
                        IsCorrect = false,
                    },
                    new Annotation() {
                        Type = AnnotationType.E2_INFINITE_LOOPS,
                        IsCorrect = false,
                    },
                    new Annotation() {
                        Type = AnnotationType.E2_THEME_FUNCTIONALITY,
                        IsCorrect = false,
                    },
                    new Annotation() {
                        Type = AnnotationType.E2_ROUTES,
                        IsCorrect = false,
                    },
                    new Annotation() {
                        Type = AnnotationType.E2_ADD_FAVS,
                        IsCorrect = false,
                    },
                    new Annotation() {
                        Type = AnnotationType.E2_KEY_ON_LIST,
                        IsCorrect = false,
                    },
                    new Annotation() {
                        Type = AnnotationType.E2_DETAIL_FETCH,
                        IsCorrect = false,
                    },
                    new Annotation() {
                        Type = AnnotationType.E2_FAVS_FUNCTIONALITY,
                        IsCorrect = false,
                    },
                    new Annotation() {
                        Type = AnnotationType.E2_CONTACT_FORM_FUNCTIONALITY,
                        IsCorrect = false,
                    },
                    
                },
            },
        };
    }
}
