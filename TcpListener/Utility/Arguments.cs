/*
* Arguments class: application arguments interpreter
*
* Authors:		R. LOPES
* Contributors:	R. LOPES
* Created:		25 October 2002
* Modified:		28 October 2002
*
* Version:		1.0
*/
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Listener.Utility
{
	/// <summary>
	/// Arguments class
	/// </summary>
	public class Arguments
    {
		// Variables
		private readonly StringDictionary parameters;
	    private readonly string command;

		// Constructor
		public Arguments(IEnumerable<string> args, string cmd = "IOT+CMD")
        {
            string parameter = null;
            parameters = new StringDictionary();
			var spliter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			var remover = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		    // Valid parameters forms:
			// {-,/,--}param{ ,=,:}((",')value(",'))
			// Examples: -param1 value1 --param2 /param3:"Test-:-work" /param4=happy -param5 '--=nice=--'
            foreach (var txt in args)
            {
                // Look for new parameters (-,/ or --) and a possible enclosed value (=,:)
                var parts = spliter.Split(txt, 3);

                switch (parts.Length)
			    {
			        // Found a value (for the last parameter found (space separator))
			        case 1:
			            if(parameter != null)
			            {
			                if(!parameters.ContainsKey(parameter))
			                {
			                    parts[0] = remover.Replace(parts[0], "$1");
			                    parameters.Add(parameter, parts[0]);
			                }
			                parameter = null;
			            }
			            else
			            {
			                if (parts[0] == cmd) command = parts[0];
			            }
			            // else Error: no parameter waiting for a value (skipped)
			            break;

			        // Found just a parameter
			        case 2:
			            // The last parameter is still waiting. With no value, set it to true.
			            if(parameter != null)
			            {
			                if(!parameters.ContainsKey(parameter)) parameters.Add(parameter, "true");
			            }
			            parameter = parts[1];
			            break;

			        // Parameter with enclosed value
			        case 3:
			            // The last parameter is still waiting. With no value, set it to true.
			            if(parameter != null)
			            {
			                if(!parameters.ContainsKey(parameter)) parameters.Add(parameter, "true");
			            }
			            parameter = parts[1];
			            // Remove possible enclosing characters (",')
			            if(!parameters.ContainsKey(parameter))
			            {
			                parts[2] = remover.Replace(parts[2], "$1");
			                parameters.Add(parameter, parts[2]);
			            }
			            parameter = null;
			            break;
			    }
			}
		    // In case a parameter is still waiting
		    if (parameter == null) return;
		    if(!parameters.ContainsKey(parameter)) parameters.Add(parameter, "true");
        }

		// Retrieve a parameter value if it exists
		public string this [string param]
        {
			get { return(parameters[param]); }
		}

        // Retrieve command
	    public string Command
	    {
            get { return command; }
	    }
	}
}
