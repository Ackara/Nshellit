﻿using System;

namespace Acklann.NShellit.Attributes
{
    /// <summary>
    /// Defines a parameter/argument for a command. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage((AttributeTargets.Property | AttributeTargets.Field), AllowMultiple = false, Inherited = true)]
    public sealed class ParameterAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterAttribute"/> class.
        /// </summary>
        /// <param name="aliases">The command aliases.</param>
        public ParameterAttribute(params string[] aliases)
        {
            Position = -1;
            Aliases = aliases;
        }

        /// <summary>
        /// The aliases
        /// </summary>
        public readonly string[] Aliases;

        /// <summary>
        /// Gets or sets the ordinal position.
        /// </summary>
        /// <value>The position.</value>
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets the help text.
        /// </summary>
        /// <value>The help text.</value>
        public string HelpText { get; set; }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>The default value.</value>
        public object Default { get; set; }

        /// <summary>
        /// Gets or sets the kind of data.
        /// </summary>
        /// <value>The kind of data.</value>
        public string Kind { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ParameterAttribute"/> is required.
        /// </summary>
        /// <value><c>true</c> if required; otherwise, <c>false</c>.</value>
        public bool Required { get; set; }
    }
}