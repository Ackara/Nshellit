﻿using System;
using System.Collections.Generic;

namespace Acklann.NShellit.Extensions
{
    /// <summary>
	/// Provides methods that maps the command-line argument array to an object.
	/// </summary>
    public static partial class MapExtensions
    {
        /// <summary>
        /// Maps the specified <paramref name="arguments"/> to a command within the <paramref name="commandList"/>.
        /// </summary>
        /// <typeparam name="TCommand">The type of the t command.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="arguments">The argument array.</param>
        /// <param name="commandList">The command list.</param>
        /// <param name="onSuccess">The action invoked when a command is found.</param>
        /// <param name="onParsingError">The action invoked when a parsing error occurs.</param>
        /// <exception cref="ArgumentException"><typeparamref name="TCommand"/> is not assignable from the mapped object command.</exception>
        public static void Map<TCommand>(this Parser parser, string[] arguments, IEnumerable<Type> commandList, Action<TCommand> onSuccess, Action<string> onParsingError)
        {
            foreach (Type type in commandList) parser.Add(CommandInfo.ConvertFrom(type));

            if (parser.TryMap(arguments, out CommandInfo command, out string error))
            {
                if (command.IsInternal) { /* DO NOTHING; IT'S ALREADY HANDLED. */ }
                else if (typeof(TCommand).IsAssignableFrom(command.Target) == false) throw new ArgumentException($"{command.Target.Name} is not of type {typeof(TCommand).Name}.", nameof(commandList));
                else onSuccess((TCommand)command.ToObject());
            }
            else onParsingError?.Invoke(error);
        }

        /// <summary>
        /// Maps the specified <paramref name="arguments"/> to a command within the <paramref name="commandList"/>.
        /// </summary>
        /// <typeparam name="TCommand">The type of the t command.</typeparam>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="arguments">The arguments array.</param>
        /// <param name="commandList">The command list.</param>
        /// <param name="onSuccess">The func invoked when a command is found.</param>
        /// <param name="onParsingError">The func invoked when a parsing error occurs.</param>
        /// <returns>TResult.</returns>
        /// <exception cref="ArgumentException">commandList</exception>
        public static TResult Map<TCommand, TResult>(this Parser parser, string[] arguments, IEnumerable<Type> commandList, Func<TCommand, TResult> onSuccess, Func<string, TResult> onParsingError)
        {
            foreach (Type type in commandList) parser.Add(CommandInfo.ConvertFrom(type));

            if (parser.TryMap(arguments, out CommandInfo command, out string error))
            {
                if (command.IsInternal) return default(TResult);
                else if (typeof(TCommand).IsAssignableFrom(command.Target) == false) throw new ArgumentException($"{command.Target.Name} is not of type {typeof(TCommand).Name}.", nameof(commandList));
                else return onSuccess((TCommand)command.ToObject());
            }
            else if (onParsingError != null) return onParsingError(error);
            else return default(TResult);
        }

        internal static void Map(this Parser parser, string[] args, Action<string> onParsingError, ValueTuple<Type, Action<object>>[] commandList)
        {
            var map = new Dictionary<string, Action<object>>(commandList.Length);
            foreach ((Type Type, Action<object> Callback) command in commandList)
            {
                map.Add(command.Type.Name, command.Callback);
                parser.Add(CommandInfo.ConvertFrom(command.Type));
            }

            if (parser.TryMap(args, out CommandInfo result, out string error))
            {
                if (map.ContainsKey(result.Target.Name ?? string.Empty)) map[result.Target.Name](result.ToObject());
            }
            else onParsingError?.Invoke(error);
        }

        internal static TResult Map<TResult>(this Parser parser, string[] args, Func<string, TResult> onParsingError, ValueTuple<Type, Func<object, TResult>>[] commandList)
        {
            var map = new Dictionary<string, Func<object, TResult>>(commandList.Length);
            foreach ((Type Type, Func<object, TResult> Callback) command in commandList)
            {
                map.Add(command.Type.Name, command.Callback);
                parser.Add(CommandInfo.ConvertFrom(command.Type));
            }

            if (parser.TryMap(args, out CommandInfo result, out string error))
            {
                string key = result.Target.Name;
                if (map.ContainsKey(key)) return map[key](result.ToObject());
                else return default(TResult);
            }
            else if (onParsingError != null) return onParsingError(error);
            else return default(TResult);
        }
    }
}