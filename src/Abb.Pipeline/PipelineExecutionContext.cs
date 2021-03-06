﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Abb.Pipeline
{
    public class PipelineExecutionContext : IPipelineExecutionContext
    {
        private readonly IList<(string Name, Type ValueType, object Value)> _variables = new List<(string Name, Type ValueType, object Value)>();

        public object CurrentStep { get; set; }

        public string[] Names { get { return _variables.Select(i => i.Name).ToArray(); } }

        public static IPipelineExecutionContext Create() => new PipelineExecutionContext();

        public void Add<T>(string name, T value) => _variables.Add((name, typeof(T), value));

        public T Get<T>(string name)
        {
            (string Name, Type ValueType, object Value) variable = default;
            try
            {
                variable = _variables.Single(e => e.Name == name);
                return (T)variable.Value;
            }
            catch (InvalidOperationException)
            {
                throw new ArgumentException($"Context does not contain any value with name {name}");
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException($"Value with {name} is of type {variable.ValueType.ToString()}");
            }
        }
    }
}
