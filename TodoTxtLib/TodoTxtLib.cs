﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
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
        public bool Completed;
        public char Priority;
        public TodoMode ModeMode;
        public DateTime Completion;
        public DateTime Creation;
        public string Body;
        public List<string> Context = new List<string>();
        public List<string> Project = new List<string>();
        public Dictionary<string, string> Meta = new Dictionary<string, string>();
    }

    public static class TodoTxt
    {
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
                entry.ModeMode = TodoMode.Completion;
                var result = Regex.Matches(line, ".{4}-.{2}-.{2}");

                entry.Completion = DateTime.Parse(result[0].ToString());
                entry.Creation = DateTime.Parse(result[1].ToString());
                line = Regex.Replace(line, "^.{4}-.{2}-.{2} .{4}-.{2}-.{2} (.*)$", "$1");
            }
            else if (Regex.IsMatch(line, "^(.{4}-.{2}-.{2}) (.*)$"))
            {
                // Creation only
                entry.ModeMode = TodoMode.Creation;
                var result = Regex.Matches(line, "^.{4}-.{2}-.{2}");
                entry.Creation = DateTime.Parse(result[0].ToString());
                line = Regex.Replace(line, "^.{4}-.{2}-.{2} (.*)$", "$1");
            }
            else
            {
                // Nothing
                entry.ModeMode = TodoMode.None;
            }

            // Context
            foreach (var item in Regex.Matches(line, "@([^\\s]*)"))
            {
                entry.Context.Add(item.ToString().Replace("@", "").Trim());
            }

            line = Regex.Replace(line, "@([^\\s]*)", "").Trim();

            // Project
            foreach (var item in Regex.Matches(line, "\\+([^\\s]*)"))
            {
                entry.Project.Add(item.ToString().Replace("+", "").Trim());
            }

            line = Regex.Replace(line, "\\+([^\\s]*)", "").Trim();

            foreach (var item in Regex.Matches(line, "([^\\s]*?)\\:([^\\s]*)"))
            {
                var result = Regex.Match(item.ToString(), "([^\\s]*?)\\:([^\\s]*)");
                entry.Meta.Add(result.Groups[1].ToString(), result.Groups[2].ToString());
            }

            line = Regex.Replace(line, "([^\\s]*?)\\:([^\\s]*)", "").Trim();

            entry.Body = line;
            
            return entry;
        }
    }
}