using System;
using System.Collections.Generic;
using System.Text;

namespace AccountabilityInformationSystem.ConsoleSeeder.DTOs;

internal sealed class CreateProductTypeRequest
{
    public required string Name { get; set;  }
    public required string FullName { get; set; }
}
