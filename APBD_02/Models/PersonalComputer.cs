﻿using System;
using APBD_02.Exceptions;

namespace APBD_02.Models
{
    public class PersonalComputer : Device
    {
        public string OperatingSystem { get; private set; }

        public PersonalComputer(int id, string name, string os) : base(id, name)
        {
            OperatingSystem = os;
        }

        public override void TurnOn()
        {
            if (string.IsNullOrEmpty(OperatingSystem))
                throw new EmptySystemException();

            base.TurnOn();
        }

        public override string ToString()
        {
            return base.ToString() + $", OS: {OperatingSystem ?? "None"}";
        }
    }
}