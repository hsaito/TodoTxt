using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using TodoTxtLib;
using Xunit;
using TodoTxtLib;

namespace TodoTxtTest
{
    public class UnitTest1
    {
        [Fact]
        public List<TodoItem> ParsingTest()
        {
            var reader = new StreamReader("todo.txt");

            var todoList = new List<TodoItem>();

            do
            {
                var line = reader.ReadLine();
                var result = TodoTxt.Parse(line);
                Console.WriteLine(result.Body);
                todoList.Add(result);
            } while (!reader.EndOfStream);

            return todoList;
        }

        [Fact]
        public void OutputTest()
        {
            var list = ParsingTest();

            var writer = new StreamWriter("todo_output.txt") {AutoFlush = true};

            foreach (var item in list)
            {
                writer.WriteLine(TodoTxt.GenerateTodoLine(item));
            }
        }
    }
}