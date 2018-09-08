using System;
using System.Collections.Generic;
using System.IO;
using TodoTxtLib;
using Xunit;

namespace TodoTxtTest
{
    public class UnitTest1
    {
        [Fact]
        public void OutputTest()
        {
            var list = ParsingTest();

            var writer = new StreamWriter("todo_output.txt") {AutoFlush = true};

            foreach (var item in list) writer.WriteLine(TodoTxt.GenerateTodoLine(item));
        }

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
    }
}