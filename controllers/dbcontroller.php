<?php
require_once __DIR__ . '/../models/dbmodel.php';

class dbcontroller
{
    
    private $table;
    private PDO $db;

    public function __construct(PDO $db,$table)
    {
     
        $this->table = $table;
        $this->db=$db;
       
    }

    public function index()
    {
       
        $model= new dbmodel($this->db,$this->table); 

        $page = isset($_GET['page']) ? (int)$_GET['page'] : 1;
        $pageSize = 20;
     

        $items = $model->getAll($page, $pageSize);
        $table=$this->table;

        $total = $model->getTotal();
        $totalPages = ceil($total / $pageSize);

        require_once __DIR__ . '/../views/index.php';
    }

    public function detail()
    {
        
        $model= new dbmodel($this->db,$this->table); 
        $id = isset($_GET['id']) ? $_GET['id'] : 0;
        $items = $model->getById($id);
  
        require_once __DIR__ . '/../views/detail.php';
    }   
    
 
}