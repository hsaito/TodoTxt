﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TodoTxtLib
{
    public enum TodoMode
    {
        None,
        Completion,
        Creation
    }

    public class TodoItem
    {
        public string Body;
        public bool Completed;
        public DateTime Completion;
        public List<string> Context = new List<string>();
        public DateTime Creation;
        public Dictionary<string, string> Meta = new Dictionary<string, string>();
        public char Priority;
        public List<string> Project = new List<string>();
        public TodoMode TodoMode;
    }

    public static class TodoTxt
    {
        /// <summary>
        ///     Parse a string line to the TodoItem
        /// </summary>
        /// <param name="line">String line</param>
        /// <returns>TodoItem</returns>
        public static TodoItem Parse(string line)
        {
            var entry = new TodoItem();
            // Completed
            if (Regex.IsMatch(line, "^x (.*)$"))
            {
                entry.Completed = true;
                // Remove the completion mark
                line = Regex.Replace(line, "^x (.*)$", "$1");
            }
            else
            {
                entry.Completed = false;
            }

            // Priority
            if (Regex.IsMatch(line, "^\\(.\\) (.*)$"))
            {
                var priority = Regex.Match(line, "^\\((.)\\)");
                entry.Priority = priority.Groups[1].ToString()[0];
                line = Regex.Replace(line, "^\\(.\\) (.*)$", "$1");
            }

            // Completion and Creation
            if (Regex.IsMatch(line, "^(.{4}-.{2}-.{2}) (.{4}-.{2}-.{2}) (.*)$"))
            {
                // Completion and Creation
                entry.TodoMode = TodoMode.Completion;
                var result = Regex.Matches(line, ".{4}-.{2}-.{2}");

                entry.Completion = DateTime.Parse(result[0].ToString());
                entry.Creation = DateTime.Parse(result[1].ToString());
                line = Regex.Replace(line, "^.{4}-.{2}-.{2} .{4}-.{2}-.{2} (.*)$", "$1");
            }
            else if (Regex.IsMatch(line, "^(.{4}-.{2}-.{2}) (.*)$"))
            {
                // Creation only
                entry.TodoMode = TodoMode.Creation;
                var result = Regex.Matches(line, "^.{4}-.{2}-.{2}");
                entry.Creation = DateTime.Parse(result[0].ToString());
                line = Regex.Replace(line, "^.{4}-.{2}-.{2} (.*)$", "$1");
            }
            else
            {
                // Nothing
                entry.TodoMode = TodoMode.None;
            }

            // Context
            foreach (var item in Regex.Matches(line, "@([^\\s]*)"))
                entry.Context.Add(item.ToString().Replace("@", "").Trim());

            line = Regex.Replace(line, "@([^\\s]*)", "").Trim();

            // Project
            foreach (var item in Regex.Matches(line, "\\+([^\\s]*)"))
                entry.Project.Add(item.ToString().Replace("+", "").Trim());

            line = Regex.Replace(line, "\\+([^\\s]*)", "").Trim();

            // Meta
            foreach (var item in Regex.Matches(line, "([^\\s]*?)\\:([^\\s]*)"))
            {
                var result = Regex.Match(item.ToString(), "([^\\s]*?)\\:([^\\s]*)");
                entry.Meta.Add(result.Groups[1].ToString(), result.Groups[2].ToString());
            }

            line = Regex.Replace(line, "([^\\s]*?)\\:([^\\s]*)", "").Trim();

            entry.Body = line;

            return entry;
        }

        /// <summary>
        ///     Generate output string from TodoItem
        /// </summary>
        /// <param name="item">TodoItem to process</param>
        /// <returns>String representation of the TodoItem</returns>
        public static string GenerateTodoLine(TodoItem item)
        {
            var output = "";

            // Completion marker
            if (item.Completed) output += "x ";

            // Creation
            if (item.TodoMode == TodoMode.Completion)
            {
                output += item.Completion.Date.ToString("yyyy-MM-dd") + " ";
                output += item.Creation.Date.ToString("yyyy-MM-dd") + " ";
            }
            else if (item.TodoMode == TodoMode.Creation)
            {
                output += item.Creation.Date.ToString("yyyy-MM-dd") + " ";
            }

            // Body
            {
                output += item.Body + " ";
            }

            // Context
            foreach (var context in item.Context) output += "@" + context + " ";

            // Project
            foreach (var project in item.Project) output += "+" + project + " ";

            // Meta
            foreach (var meta in item.Meta) output += meta.Key + ":" + meta.Value + " ";

            return output.Trim();
        }
    }
}