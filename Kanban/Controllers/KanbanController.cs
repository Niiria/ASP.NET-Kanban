using Kanban.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = Kanban.Models.Task;

namespace Kanban.Controllers
{
    public class KanbanController : Controller
    {
        private readonly SqlContext _context;

        public KanbanController(SqlContext context)
        {
            _context = context;
        }


        public class KanbanTableModel
        {
            public List<Task> Stories { set; get; } = new();
            public List<Task> ToDo { set; get; } = new();
            public List<Task> InProgress { set; get; } = new();
            public List<Task> Testing { set; get; } = new();
            public List<Task> Done { set; get; } = new();
        }





        public IActionResult Index()
        {
            if (Kanban.Controllers.AuthController.CheckAuth())
                RedirectToAction("Index", "Home");

            ViewBag.Status = new SelectList(_context.Status.ToList(), "Id", "Name");
            var sqlContext = _context.Task.Include(t => t.Status).Include(t=>t.Users);
            KanbanTableModel KanbanTable = new();   
            foreach (var item in sqlContext)
            {
                switch (item.Status.Name)
                {
                    case "Stories":
                        KanbanTable.Stories.Add(item);
                        break;
                    case "ToDo":
                        KanbanTable.ToDo.Add(item);
                        break;
                    case "InProgress":
                        KanbanTable.InProgress.Add(item);
                        break;
                    case "Testing":
                        KanbanTable.Testing.Add(item);
                        break;
                    case "Done":
                        KanbanTable.Done.Add(item);
                        break;
                }
            }
            KanbanTable.Stories = KanbanTable.Stories.OrderBy(i => i.DueDate).ToList();
            KanbanTable.ToDo = KanbanTable.ToDo.OrderBy(i => i.DueDate).ToList();
            KanbanTable.InProgress = KanbanTable.InProgress.OrderBy(i => i.DueDate).ToList();
            KanbanTable.Testing = KanbanTable.Testing.OrderBy(i => i.DueDate).ToList();
            KanbanTable.Done = KanbanTable.Done.OrderBy(i => i.DueDate).ToList();
            return View(KanbanTable);
        }


        public IActionResult Create()
        {
            if (Kanban.Controllers.AuthController.CheckAuth())
                RedirectToAction("Index", "Home");

            ViewBag.Status = new SelectList( _context.Status.ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(Task model)
        {
           if (Kanban.Controllers.AuthController.CheckAuth())
                RedirectToAction("Index", "Home");

            Console.WriteLine(model);
            var TaskToDb = _context.Task.Include(t => t.Status).FirstOrDefault(task => task.Id == model.Id);
            if (TaskToDb != null)
            {
                switch (model.Status.Name)
                {
                    case "Done":
                        {
                            if (TaskToDb.StatusId != 5)
                                TaskToDb.StatusId += 1;
                            if (TaskToDb.StatusId == 5)
                                TaskToDb.CompletedDate = DateTime.Now;
                            TaskToDb.Status = _context.Status.Find(TaskToDb.StatusId);
                            _context.Task.Update(TaskToDb);
                            break;
                        }
                    case "Edit":
                        {
                            TaskToDb.StatusId = model.StatusId;
                            if (TaskToDb.StatusId == 5)
                                TaskToDb.CompletedDate = DateTime.Now;
                            else if (TaskToDb.CompletedDate!=null)
                            {
                                TaskToDb.CompletedDate = null;
                            }
                            TaskToDb.Subject = model.Subject;
                            TaskToDb.Status = _context.Status.Find(TaskToDb.StatusId);
                            _context.Task.Update(TaskToDb);
                            break;
                        }
                    case "Delete":
                            _context.Task.Remove(TaskToDb);
                        break;
                    default:
                        break;
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }



        [HttpPost]
        public async Task<IActionResult> Create(Task task)
        {
            if (Kanban.Controllers.AuthController.CheckAuth())
                RedirectToAction("Index", "Home");

            ViewBag.Status = new SelectList(_context.Status.ToList(), "Id", "Name");

            if (ModelState.IsValid)
            {
                var taskToDb = task;
                if(taskToDb.StartDate > taskToDb.DueDate)
                {
                    ModelState.AddModelError("StartDate", "Starting date can't excess due");
                    return View(task);
                }
                taskToDb.Users = _context.User.Where(u => u.Id == Kanban.Controllers.AuthController.LoggedUser.Id).ToList();
                if (taskToDb.Users == null)
                {
                    ModelState.AddModelError("Subject", "User not found, try login again");
                    return View(task);
                }
                taskToDb.Status = _context.Status.Find(taskToDb.StatusId);
                if (taskToDb.Status == null)
                {
                    ModelState.AddModelError("Subject", "Status not found");
                    return View(task);
                }

                taskToDb.CreatedDate = DateTime.Now;

                _context.Task.Add(taskToDb);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
           
            return View(task);
        }

    }
}
