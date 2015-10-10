




//AUTOGENERATED, DO NOTMODIFY.
//Do not edit this file directly.

#pragma warning disable 1591
// ReSharper disable CheckNamespace

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using RethinkDb.Driver.Model;
using RethinkDb.Driver.Ast;
using NUnit.Framework;
using RethinkDb.Driver.Tests;

namespace RethinkDb.Driver.Test.Generated {
    [TestFixture]
    public class Regression579 : GeneratedTest {

            public static Table tbl = r.db(DbName).table("tbl");


        [Test]
        public void YamlTest(){

             TestCounter++;
             
             {
                 //JavaQuery, regression/579.yaml, #1
                 /* ExpectedOriginal: None */
                 var expected_ = null as object;
                 
                 /* Original: tbl.insert({'name':'Jim Brown'}) */
                 var obtained = runOrCatch( tbl.insert(r.hashMap("name", "Jim Brown")) ,
                                            new OptArgs()
                    );
                 assertEquals(expected_, obtained);
             }
             
             TestCounter++;
             
             {
                 //JavaQuery, regression/579.yaml, #2
                 /* ExpectedOriginal: err("ReqlQueryLogicError", "Could not prove function deterministic.  Index functions must be deterministic.", []) */
                 var expected_ = err("ReqlQueryLogicError", "Could not prove function deterministic.  Index functions must be deterministic.", r.array());
                 
                 /* Original: tbl.index_create("579", lambda rec:r.js("1")) */
                 var obtained = runOrCatch( tbl.indexCreate("579", rec => r.js("1")) ,
                                            new OptArgs()
                    );
                 assertEquals(expected_, obtained);
             }
             
             TestCounter++;
             
             {
                 //JavaQuery, regression/579.yaml, #3
                 /* ExpectedOriginal: err("ReqlQueryLogicError", "Could not prove function deterministic.  Index functions must be deterministic.", []) */
                 var expected_ = err("ReqlQueryLogicError", "Could not prove function deterministic.  Index functions must be deterministic.", r.array());
                 
                 /* Original: tbl.index_create("579", lambda rec:tbl.get(0)) */
                 var obtained = runOrCatch( tbl.indexCreate("579", rec => tbl.get(0L)) ,
                                            new OptArgs()
                    );
                 assertEquals(expected_, obtained);
             }
             

        }
    }
}