using System.Text;

namespace WebApplicationMVC.Helpers
{
    public static class BreadcrumbHelper
    {
        public static string GetBreadcrumb(string controller, string action)
        {
            // Главная и авторизация — без хлебных крошек
            if ((controller == "Home" && action == "Index") || (controller == "Auth" && action == "Login"))
                return string.Empty;

            var sb = new StringBuilder();

            // Если текущая страница — один из личных кабинетов и это Index
            if ((controller == "Admin" || controller == "Parent" || controller == "Teacher") && action == "Index")
            {
                sb.Append("Личный кабинет");
            }
            else
            {
                // Если мы внутри личного кабинета
                if (IsInsidePersonalArea(controller))
                {
                    sb.Append("Личный кабинет");
                }

                var controllerName = GetReadableControllerName(controller);

                if (!string.IsNullOrEmpty(controllerName))
                {
                    sb.Append(" / ");
                    sb.Append(controllerName);
                }

                if (action != "Index")
                {
                    sb.Append(" / ");
                    sb.Append(GetReadableActionName(action));
                }
            }

            return sb.ToString();
        }

        private static bool IsInsidePersonalArea(string controller)
        {
            // Находимся ли мы в одном из личных кабинетов или их дочерних разделах
            return controller == "Admin" || controller == "Parent" || controller == "Teacher" ||
                   controller == "Courses" || controller == "Groups" || controller == "Students" ||
                   controller == "CalendarLesson" || controller == "Users";
        }

        private static string GetReadableControllerName(string controller)
        {
            return controller switch
            {
                "Courses" => "Курсы",
                "Groups" => "Группы",
                "Students" => "Ученики",
                "CalendarLesson" => "Расписание",
                "Users" => "Пользователи",

                // НЕ отображаем названия самих личных кабинетов
                "Admin" or "Parent" or "Teacher" => string.Empty,

                _ => controller
            };
        }

        private static string GetReadableActionName(string action)
        {
            return action switch
            {
                "Create" => "Создать",
                "Edit" => "Редактировать",
                "Details" => "Детали",
                "Delete" => "Удалить",
                "MySchedule" => "Расписание",
                "ChildSchedule" => "Расписание",
                "MyChilds" => "Дети",
                "AddAttendances" => "Посещаемость",
                _ => action
            };
        }
    }
}
