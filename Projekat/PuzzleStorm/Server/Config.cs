﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Server
{
    public static class Config
    {
        public static string ConnectionString => ConfigurationManager.ConnectionStrings["RabbitMQConnection"].ConnectionString;
        public static int DefaultWorkerPoolSize => 1;

    }
}
