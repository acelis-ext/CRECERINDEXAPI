using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CrecerIndex.Abstraction.DependencyInjection
{
    public static class RepositorySearchService
    {
        public static void RegistrarRepository(IServiceCollection services)
        {
            string directorioBase = AppDomain.CurrentDomain.BaseDirectory;
            string[] archivosEnsamblado = Directory.GetFiles(directorioBase, "*.dll", SearchOption.AllDirectories);

            foreach (var archivoEnsamblado in archivosEnsamblado)
            {
                try
                {
                    Assembly ensamblado = Assembly.LoadFrom(archivoEnsamblado);

                    var repositorios = ensamblado.GetTypes()
                        .Where(type => type.Namespace != null && type.Namespace.StartsWith("CrecerIndex.Repository"))
                        .ToList();

                    foreach (var repositorio in repositorios)
                    {
                        if (repositorio.IsClass)
                        {
                            var interfacesImplementadas = repositorio.GetInterfaces()
                                .Where(i => i.Namespace != null && i.Namespace.StartsWith("CrecerIndex.Abstraction.Interfaces.IRepository"))
                                .ToList();

                            foreach (var interfaz in interfacesImplementadas)
                            {
                                services.AddScoped(interfaz, repositorio);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al cargar el ensamblado {archivoEnsamblado}: {ex.Message}");
                }
            }
        }

        public static void RegistrarServices(IServiceCollection services)
        {
            string directorioBase = AppDomain.CurrentDomain.BaseDirectory;
            string[] archivosEnsamblado = Directory.GetFiles(directorioBase, "*.dll", SearchOption.AllDirectories);

            foreach (var archivoEnsamblado in archivosEnsamblado)
            {
                try
                {
                    Assembly ensamblado = Assembly.LoadFrom(archivoEnsamblado);

                    var servicios = ensamblado.GetTypes()
                        .Where(type => type.Namespace != null && type.Namespace.StartsWith("CrecerIndex.Services"))
                        .ToList();

                    foreach (var servicio in servicios)
                    {
                        if (servicio.IsClass)
                        {
                            var interfacesImplementadas = servicio.GetInterfaces()
                                .Where(i => i.Namespace != null && i.Namespace.StartsWith("CrecerIndex.Abstraction.Interfaces.IServices"))
                                .ToList();

                            foreach (var interfaz in interfacesImplementadas)
                            {
                                services.AddScoped(interfaz, servicio);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al cargar el ensamblado {archivoEnsamblado}: {ex.Message}");
                }
            }
        }
    }
}
