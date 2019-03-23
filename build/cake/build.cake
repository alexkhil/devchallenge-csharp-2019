///////////////////////////////////////////////////////////////////////////////
// UTILS SCRIPTS
///////////////////////////////////////////////////////////////////////////////
#load Utils/paths.cake

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument<string>("Target", "Default");
var configuration = Argument<string>("Configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

#load Tasks/run-unit-tests.cake
#load Tasks/run-integration-tests.cake

///////////////////////////////////////////////////////////////////////////////

RunTarget(target);